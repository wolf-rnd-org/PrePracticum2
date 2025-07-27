using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FFmpeg.API.DTOs;
using FFmpeg.Core.Interfaces;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace FFmpeg.API.Endpoints
{
    public static class VideoEndpoints
    {
        public static void MapEndpoints(this WebApplication app)
        {
            app.MapPost("/api/video/watermark", AddWatermark)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600)); // 100 MB

            app.MapPost("/api/video/crop", CropVideo)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600)); // 100 MB
        }

        private static async Task<IResult> AddWatermark(HttpContext context, [FromForm] WatermarkDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                if (dto.VideoFile == null || dto.WatermarkFile == null)
                {
                    return Results.BadRequest("Video file and watermark file are required");
                }

                string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);
                string watermarkFileName = await fileService.SaveUploadedFileAsync(dto.WatermarkFile);

                string extension = Path.GetExtension(dto.VideoFile.FileName);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);

                List<string> filesToCleanup = new List<string> { videoFileName, watermarkFileName, outputFileName };

                try
                {
                    var command = ffmpegService.CreateWatermarkCommand();
                    var result = await command.ExecuteAsync(new WatermarkModel
                    {
                        InputFile = videoFileName,
                        WatermarkFile = watermarkFileName,
                        OutputFile = outputFileName,
                        XPosition = dto.XPosition,
                        YPosition = dto.YPosition,
                        IsVideo = true,
                        VideoCodec = "libx264"
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg command failed: {ErrorMessage}, Command: {Command}",
                            result.ErrorMessage, result.CommandExecuted);
                        return Results.Problem("Failed to add watermark: " + result.ErrorMessage, statusCode: 500);
                    }

                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);

                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    return Results.File(fileBytes, "video/mp4", dto.VideoFile.FileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing watermark request");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AddWatermark endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }


        private static async Task<IResult> CropVideo(HttpContext context, [FromForm] CropDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                if (dto.VideoFile == null)
                {
                    return Results.BadRequest("Video file is required");
                }

                string inputFile = await fileService.SaveUploadedFileAsync(dto.VideoFile);
                string outputFile = await fileService.GenerateUniqueFileNameAsync(Path.GetExtension(dto.VideoFile.FileName));

                var filesToCleanup = new List<string> { inputFile, outputFile };

                var request = new CropRequest
                {
                    InputPath = inputFile,
                    OutputPath = outputFile,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    Width = dto.Width,
                    Height = dto.Height,
                    X = dto.X,
                    Y = dto.Y
                };

                var command = new CropCommand();
                var result = await command.RunAsync(request);

                if (!result.IsSuccess)
                {
                    logger.LogError("Crop failed: {Error}", result.Error);
                    return Results.Problem("Crop failed: " + result.Error, statusCode: 500);
                }

                var fileBytes = await fileService.GetOutputFileAsync(outputFile);
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                return Results.File(fileBytes, "video/mp4", "cropped_" + dto.VideoFile.FileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in crop endpoint");
                return Results.Problem("Error: " + ex.Message, statusCode: 500);
            }
        }
    }

}