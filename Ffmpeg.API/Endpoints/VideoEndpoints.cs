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

            app.MapPost("/api/video/replace-audio", ReplaceAudio)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600));

            app.MapPost("/api/video/resize", ChangeResolution)
               .DisableAntiforgery()
               .WithMetadata(new RequestSizeLimitAttribute(104857600));

            // ----------- AUDIO ENDPOINT -----------
            app.MapPost("/api/video/timestamp", AddTimestamp)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600));

            app.MapPost("/api/audio/convert", ConvertAudio)
                .DisableAntiforgery()
                .WithName("ConvertAudio")
                .Accepts<ConvertAudioDto>("multipart/form-data");
        }

        private static async Task<IResult> AddWatermark(
            HttpContext context,
            [FromForm] WatermarkDto dto)
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

                List<string> filesToCleanup = new() { videoFileName, watermarkFileName, outputFileName };

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

        private static async Task<IResult> ReplaceAudio(
            HttpContext context,
            [FromForm] ReplaceAudioDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                if (dto.VideoFile == null || dto.NewAudioFile == null)
                {
                    return Results.BadRequest("Both video file and new audio file are required.");
                }

                string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);
                string audioFileName = await fileService.SaveUploadedFileAsync(dto.NewAudioFile);
                string extension = Path.GetExtension(dto.VideoFile.FileName);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);

                List<string> filesToCleanup = new List<string> { videoFileName, audioFileName, outputFileName };

                try
                {
                    var command = ffmpegService.CreateReplaceAudioCommand();
                    var result = await command.ExecuteAsync(new ReplaceAudioModel
                    {
                        VideoFile = videoFileName,
                        NewAudioFile = audioFileName,
                        OutputFile = outputFileName
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg command failed: {ErrorMessage}, Command: {Command}",
                            result.ErrorMessage, result.CommandExecuted);
                        return Results.Problem("Failed to replace audio: " + result.ErrorMessage, statusCode: 500);
                    }

                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    return Results.File(fileBytes, "video/mp4", dto.VideoFile.FileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing replace-audio request");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in ReplaceAudio endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> ChangeResolution(
             HttpContext context,
            [FromForm] ResizeDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                if (dto.InputFile == null || dto.Width <= 0 || dto.Height <= 0)
                {
                    return Results.BadRequest("Video file and valid resolution (Width and Height) are required.");
                }

                // Save the uploaded video
                string inputFilePath = await fileService.SaveUploadedFileAsync(dto.InputFile);

                // Determine output file path
                string outputFileName = string.IsNullOrWhiteSpace(dto.OutputFile)
                ? await fileService.GenerateUniqueFileNameAsync(".mp4")
                : Path.HasExtension(dto.OutputFile) ? dto.OutputFile : dto.OutputFile + ".mp4";

                var filesToCleanup = new List<string> { inputFilePath, outputFileName };

                try
                {
                    // Create and execute FFmpeg resize command
                    var command = ffmpegService.CreateResizeCommand(); // Must return IFFmpegResizeCommand
                    var result = await command.ExecuteAsync(new ResizeModel
                    {
                        InputFile = inputFilePath,
                        OutputFile = outputFileName,
                        Width = dto.Width,
                        Height = dto.Height,
                        VideoCodec = "libx264"
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg resize failed: {ErrorMessage}. Command: {CommandExecuted}",
                            result.ErrorMessage, result.CommandExecuted);
                        return Results.Problem("Failed to resize video: " + result.ErrorMessage, statusCode: 500);
                    }

                    byte[] outputBytes = await fileService.GetOutputFileAsync(outputFileName);

                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    return Results.File(outputBytes, "video/mp4", Path.GetFileName(outputFileName));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error executing FFmpeg resize command");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled error in ChangeResolution endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }

        // ---------- AUDIO ----------
        private static async Task<IResult> ConvertAudio(
            HttpContext context,
            [FromForm] ConvertAudioDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            if (dto.AudioFile == null || string.IsNullOrEmpty(dto.OutputFileName))
            {
                return Results.BadRequest("Audio file and output name are required");
            }

            string inputFileName = await fileService.SaveUploadedFileAsync(dto.AudioFile);
            string extension = Path.GetExtension(dto.OutputFileName);
            if (string.IsNullOrEmpty(extension))
            {
                return Results.BadRequest("Output file name must include extension (e.g., .wav)");
            }

            string outputFileName = dto.OutputFileName;
            List<string> filesToCleanup = new() { inputFileName, outputFileName };

            try
            {
                var command = ffmpegService.CreateConvertAudioCommand();
                var result = await command.ExecuteAsync(new ConvertAudioModel
                {
                    InputFile = inputFileName,
                    OutputFile = outputFileName
                });

                if (!result.IsSuccess)
                {
                    logger.LogError("Audio conversion failed: {Error}", result.ErrorMessage);
                    return Results.Problem("Audio conversion failed: " + result.ErrorMessage);
                }

                byte[] output = await fileService.GetOutputFileAsync(outputFileName);
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                return Results.File(output, "audio/wav", outputFileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error converting audio");
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                return Results.Problem("Unexpected error: " + ex.Message);
            }
        }

        private static async Task<IResult> AddTimestamp(
            HttpContext context,
            [FromForm] TimestampDto dto)
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
                string extension = Path.GetExtension(dto.VideoFile.FileName);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);

                List<string> filesToCleanup = new List<string> { videoFileName, outputFileName };

                try
                {
                    var command = ffmpegService.CreateTimestampCommand();
                    var result = await command.ExecuteAsync(new TimestampModel
                    {
                        InputFile = videoFileName,
                        OutputFile = outputFileName,
                        XPosition = dto.XPosition == 0 ? 10 : dto.XPosition,
                        YPosition = dto.YPosition == 0 ? 10 : dto.YPosition,
                        FontSize = dto.FontSize == 0 ? 10 : dto.FontSize,
                        FontColor = string.IsNullOrWhiteSpace(dto.FontColor) ? "white" : dto.FontColor,
                        IsVideo = true,
                        VideoCodec = "libx264"
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg timestamp command failed: {ErrorMessage}, Command: {Command}",
                            result.ErrorMessage, result.CommandExecuted);
                        return Results.Problem("Failed to add timestamp: " + result.ErrorMessage, statusCode: 500);
                    }

                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    return Results.File(fileBytes, "video/mp4", dto.VideoFile.FileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing timestamp request");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AddTimestamp endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }
    }
}

