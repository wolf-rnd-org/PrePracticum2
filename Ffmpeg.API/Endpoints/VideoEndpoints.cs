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
using System.Diagnostics;
using FFmpeg.Infrastructure.Commands;


namespace FFmpeg.API.Endpoints
{
    public static class VideoEndpoints 
    {
        private const int MaxUploadSize = 104_857_600;

        public static void MapEndpoints(this WebApplication app)
        {

            app.MapPost("/api/video/watermark", AddWatermark)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600)); // 100 MB
          
            app.MapPost("/api/video/blurEffect", AddBlurEffect)
               .DisableAntiforgery()
               .WithMetadata(new RequestSizeLimitAttribute(104857600)); // 100 MB

            app.MapPost("api/video/thumbnail", CreateThumbnail)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(MaxUploadSize));

            app.MapPost("/api/video/greenscreen", ReplaceGreenScreen)
                .DisableAntiforgery()
                .Accepts<GreenScreenDto>("multipart/form-data");

            app.MapPost("/api/video/colorfilter", ApplyColorFilter)
                .DisableAntiforgery()
                .WithName("ApplyColorFilter")
                .WithMetadata(new RequestSizeLimitAttribute(MaxUploadSize));

            app.MapPost("/api/video/replace-audio", ReplaceAudio)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600));

            app.MapPost("/api/video/reverseVideo", ReverseVideo)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600)); // 100 MB

            app.MapPost("/api/video/timestamp", AddTimestamp)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600));

            app.MapPost("/api/audio/convert", ConvertAudio)
                .DisableAntiforgery()
                .WithName("ConvertAudio")
                .Accepts<ConvertAudioDto>("multipart/form-data")
                .WithMetadata(new RequestSizeLimitAttribute(MaxUploadSize));

            app.MapPost("/api/video/brightness-contrast", AdjustBrightnessContrast)
               .DisableAntiforgery()
               .WithMetadata(new RequestSizeLimitAttribute(MaxUploadSize));

            app.MapPost("/api/video/resize", ChangeResolution)
               .DisableAntiforgery()
               .WithMetadata(new RequestSizeLimitAttribute(MaxUploadSize));
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

        private static async Task<IResult> ReplaceGreenScreen(
                HttpContext context,
                [FromForm] GreenScreenDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                if (dto.InputFile == null || dto.BackgroundFile == null)
                {
                    return Results.BadRequest("Input file and background file are required");
                }

                string originalFileName = Path.GetFileName(dto.InputFile.FileName);
                string inputFileName = await fileService.SaveUploadedFileAsync(dto.InputFile);
                string backgroundFileName = await fileService.SaveUploadedFileAsync(dto.BackgroundFile);
                string extension = Path.GetExtension(originalFileName);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);

                List<string> filesToCleanup = new() { inputFileName, backgroundFileName, outputFileName };

                try
                {
                    var command = ffmpegService.CreateGreenScreenCommand();
                    var result = await command.ExecuteAsync(new GreenScreenModel
                    {
                        InputFile = inputFileName,
                        BackgroundFile = backgroundFileName,
                        OutputFile = outputFileName,
                        Similarity = dto.Similarity ?? 0.1,
                        Blend = dto.Blend ?? 0.2,
                        ChromaColor = string.IsNullOrWhiteSpace(dto.ChromaColor) ? "0x00FF00" : dto.ChromaColor,
                        VideoCodec = "libx264"
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg command failed: {ErrorMessage}, Command: {Command}",
                            result.ErrorMessage, result.CommandExecuted);
                        return Results.Problem("Failed to replace green screen: " + result.ErrorMessage, statusCode: 500);
                    }

                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    return Results.File(fileBytes, "video/mp4", originalFileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing green screen replacement");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in ReplaceGreenScreen endpoint");
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
        private static async Task<IResult> ApplyColorFilter(
   HttpContext context,
   [FromForm] ColorFilterDto dto)
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
                string outputFileName = string.IsNullOrEmpty(dto.OutputFileName)
                    ? await fileService.GenerateUniqueFileNameAsync(extension)
                    : dto.OutputFileName;

                List<string> filesToCleanup = new() { videoFileName, outputFileName };

                try
                {
                    var command = ffmpegService.CreateColorFilterCommand();
                    var result = await command.ExecuteAsync(new ColorFilterModel
                    {
                        InputFile = videoFileName,
                        OutputFile = outputFileName,
                        IsVideo = true,
                        VideoCodec = "libx264"
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg color filter command failed: {ErrorMessage}, Command: {Command}",
                            result.ErrorMessage, result.CommandExecuted);
                        return Results.Problem("Failed to apply color filter: " + result.ErrorMessage, statusCode: 500);
                    }

                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    return Results.File(fileBytes, "video/mp4", dto.VideoFile.FileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing color filter request");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in ApplyColorFilter endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }
        private static async Task<IResult> AddBlurEffect(
HttpContext context,
[FromForm] BlurEffectDto dto)
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
                    var command = ffmpegService.CreateBlurEffectCommand();
                    var result = await command.ExecuteAsync(new BlurEffectModel
                    {
                        VideoName = videoFileName,
                        OutputName = outputFileName,
                        Sigma = dto.Sigma // Passed from the form
                    });
                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg command failed: {ErrorMessage}, Command: {Command}",
                        result.ErrorMessage, result.CommandExecuted);
                        return Results.Problem("Failed to apply blur effect: " + result.ErrorMessage, statusCode: 500);
                    }
                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    return Results.File(fileBytes, "video/mp4", dto.VideoFile.FileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing blur effect request");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in ApplyBlurEffect endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
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
        private static async Task<IResult> AdjustBrightnessContrast(HttpContext context, [FromForm] BrightnessContrastDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            if (dto.VideoFile == null || dto.VideoFile.Length == 0)
            {
                return Results.BadRequest("Video file is required");
            }

            // שמירת קובץ הווידאו שהועלה
            string inputFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);
            string inputFilePath = fileService.GetFullInputPath(inputFileName);

            // יצירת שם קובץ פלט ייחודי
            string extension = Path.GetExtension(dto.VideoFile.FileName);
            string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);
            string outputFilePath = fileService.GetFullOutputPath(outputFileName);

            List<string> filesToCleanup = new() { inputFileName, outputFileName };

            try
            {
                string ffmpegArgs = $"-i \"{inputFilePath}\" -vf eq=brightness={dto.Brightness}:contrast={dto.Contrast} -c:a copy \"{outputFilePath}\"";

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = ffmpegArgs,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string stderr = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    logger.LogError("FFmpeg brightness/contrast command failed: {Error}", stderr);
                    return Results.Problem("Failed to adjust brightness/contrast: " + stderr, statusCode: 500);
                }

                // קריאת קובץ הפלט
                byte[] outputBytes = await fileService.GetOutputFileAsync(outputFileName);

                // ניקוי הקבצים הזמניים
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                return Results.File(outputBytes, "video/mp4", $"edited_{dto.VideoFile.FileName}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing brightness/contrast request");
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                return Results.Problem("An unexpected error occurred: " + ex.Message, statusCode: 500);
            }
        }
        private static async Task<IResult> ReverseVideo(
    HttpContext context,
    [FromForm] ReverseVideoDto dto)
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

                string inputFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);

                string extension = Path.GetExtension(dto.VideoFile.FileName);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);

                List<string> filesToCleanup = new() { inputFileName, outputFileName };

                try
                {
                    var command = ffmpegService.ReverseVideoCommand();
                    var result = await command.ExecuteAsync(new ReverseVideoModel
                    {
                        InputFile = inputFileName,
                        OutputFile = outputFileName
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg reverse failed: {ErrorMessage}, Command: {Command}",
                            result.ErrorMessage, result.CommandExecuted);
                        return Results.Problem("Failed to reverse video: " + result.ErrorMessage, statusCode: 500);
                    }

                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);

                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    return Results.File(fileBytes, "video/mp4", "reversed_" + dto.VideoFile.FileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing reverse video");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in ReverseVideo endpoint");
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
    }
}
