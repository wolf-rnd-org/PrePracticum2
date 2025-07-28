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

            const int MaxUploadSize = 104857600;
            app.MapPost("api/video/thumbnail", CreateThumbnail)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(MaxUploadSize)); // 100 MB
        }

        private static async Task<IResult> AddWatermark(
            HttpContext context,
            [FromForm] WatermarkDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>(); // or a specific logger type

            try
            {
                // Validate request
                if (dto.VideoFile == null || dto.WatermarkFile == null)
                {
                    return Results.BadRequest("Video file and watermark file are required");
                }

                // Save uploaded files
                string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);
                string watermarkFileName = await fileService.SaveUploadedFileAsync(dto.WatermarkFile);

                // Generate output filename
                string extension = Path.GetExtension(dto.VideoFile.FileName);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);

                // Track files to clean up
                List<string> filesToCleanup = new List<string> { videoFileName, watermarkFileName, outputFileName };

                try
                {
                    // Create and execute the watermark command
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

                    // Read the output file
                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);

                    // Clean up temporary files
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    // Return the file
                    return Results.File(fileBytes, "video/mp4", dto.VideoFile.FileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing watermark request");
                    // Clean up on error
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

        private static async Task<IResult> CreateThumbnail(
        HttpContext context,
        [FromForm] ThumbnailDto dto)
            {
                var fileService = context.RequestServices.GetRequiredService<IFileService>();
                var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

                try
                {
                    if (dto.VideoFile == null)
                    {
                        return Results.BadRequest("Video file is required");
                    }

                    string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);
                    string outputFileName = await fileService.GenerateUniqueFileNameAsync(".jpg");

                    List<string> filesToCleanup = new() { videoFileName, outputFileName };

                    try
                    {
                        var command = ffmpegService.CreateThumbnailCommand();
                        var result = await command.ExecuteAsync(new ThumbnailModel
                        {
                            VideoFile = videoFileName,
                            OutputFile = outputFileName
                        });

                        if (!result.IsSuccess)
                        {
                            logger.LogError("Thumbnail generation failed: {ErrorMessage}, Command: {Command}",
                                result.ErrorMessage, result.CommandExecuted);
                            return Results.Problem("Failed to generate thumbnail: " + result.ErrorMessage, statusCode: 500);
                        }

                        byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);
                        _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                        return Results.File(fileBytes, "image/jpeg", outputFileName);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error processing thumbnail request");
                        _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in GenerateThumbnail endpoint");
                    return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
                }
            }

    }
}