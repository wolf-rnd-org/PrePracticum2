using Ffmpeg.Command;
using FFmpeg.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FFmpeg.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly string _basePath;
        private readonly string _inputPath;
        private readonly string _outputPath;
        private readonly string _tempPath;

        public FileService(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;

            _basePath = Environment.ExpandEnvironmentVariables(
                _configuration["FFmpeg:Path"] ?? throw new InvalidOperationException("FFmpeg:Path configuration is missing"));

            _inputPath = Path.Combine(_basePath, "Input");
            _outputPath = Path.Combine(_basePath, "Output");
            _tempPath = Path.Combine(_basePath, "Temp");

            // Ensure directories exist
            Directory.CreateDirectory(_inputPath);
            Directory.CreateDirectory(_outputPath);
            Directory.CreateDirectory(_tempPath);
        }

        /// <summary>
        /// Saves an uploaded file to the input directory
        /// </summary>
        public async Task<string> SaveUploadedFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty", nameof(file));
            }

            string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string fileName = await GenerateUniqueFileNameAsync(fileExtension);
            string filePath = Path.Combine(_inputPath, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation($"Saved file {fileName} ({file.Length} bytes)");
                return fileName;
            }
            catch (Exception ex)
            {

                _logger.LogInformation($"Saved file {fileName} ({file.Length} bytes)");
                _logger.LogError(ex, $"Error saving file {fileName}");
                throw new IOException($"Error saving file: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the full path to a file in the input directory
        /// </summary>
        public string GetFullInputPath(string fileName)
        {
            return Path.Combine(_inputPath, fileName);
        }

        /// <summary>
        /// Gets the full path to a file in the output directory
        /// </summary>
        public string GetFullOutputPath(string fileName)
        {
            return Path.Combine(_outputPath, fileName);
        }

        /// <summary>
        /// Gets the full path to a file in the temp directory
        /// </summary>
        public string GetFullTempPath(string fileName)
        {
            return Path.Combine(_tempPath, fileName);
        }

        /// <summary>
        /// Reads an output file as a byte array
        /// </summary>
        public async Task<byte[]> GetOutputFileAsync(string fileName)
        {
            string filePath = GetFullOutputPath(fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Output file not found: {fileName}");
            }

            try
            {
                return await File.ReadAllBytesAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reading output file {fileName}");
                throw new IOException($"Error reading output file: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cleans up temporary files
        /// </summary>
        public async Task CleanupTempFilesAsync(IEnumerable<string> fileNames)
        {
            foreach (var fileName in fileNames)
            {
                try
                {
                    // Check both input and output paths
                    string inputFile = GetFullInputPath(fileName);
                    if (File.Exists(inputFile))
                    {
                        File.Delete(inputFile);
                    }

                    string outputFile = GetFullOutputPath(fileName);
                    if (File.Exists(outputFile))
                    {
                        File.Delete(outputFile);
                    }

                    string tempFile = GetFullTempPath(fileName);
                    if (File.Exists(tempFile))
                    {
                        File.Delete(tempFile);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Error cleaning up file {fileName}");
                    // Continue with other files even if one fails
                }
            }

            // Allow any async cleanup operations to complete
            await Task.CompletedTask;
        }

        /// <summary>
        /// Generates a unique filename with a timestamp and random component
        /// </summary>
        public async Task<string> GenerateUniqueFileNameAsync(string extension)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string random = Guid.NewGuid().ToString("N").Substring(0, 8);
            string fileName = $"{timestamp}_{random}{extension}";

            // Ensure filename is unique
            await Task.CompletedTask;
            return fileName;
        }
    }
}
