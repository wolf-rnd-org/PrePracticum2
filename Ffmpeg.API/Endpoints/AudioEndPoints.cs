using FFmpeg.API.DTOs;

namespace FFmpeg.API.Endpoints
{
    public class AudioEndPoints
    {
        public static void MapAudioEndpoints(this WebApplication app)
        {
            app.MapPost("/api/audio/mix", MixAudio)
            .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(104857600));
        }
        private static async Task<IResult> MixAudio(HttpContext context, [FromForm] AudioMixDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                if (dto.AudioFile1 == null || dto.AudioFile2 == null)
                {
                    return Results.BadRequest("Both audio files are required");
                }

                string file1 = await fileService.SaveUploadedFileAsync(dto.AudioFile1);
                string file2 = await fileService.SaveUploadedFileAsync(dto.AudioFile2);
                string outputFile = await fileService.GenerateUniqueFileNameAsync(".mp3");

                List<string> filesToCleanup = new List<string> { file1, file2, outputFile };

                try
                {
                    var command = ffmpegService.CreateMixAudioCommand();
                    var result = await command.ExecuteAsync(new AudioMixModel
                    {
                        InputFile1 = fileService.GetFullInputPath(file1),
                        InputFile2 = fileService.GetFullInputPath(file2),
                        OutputFile = fileService.GetFullOutputPath(outputFile)
                    });

                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg mix command failed: {ErrorMessage}, Command: {Command}",
                            result.ErrorMessage, result.CommandExecuted);
                        return Results.Problem("Failed to mix audio: " + result.ErrorMessage, statusCode: 500);
                    }

                    var fileBytes = await fileService.GetOutputFileAsync(outputFile);
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                    return Results.File(fileBytes, "audio/mpeg", "mixed-audio.mp3");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error mixing audio");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in MixAudio endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }
    }
}
