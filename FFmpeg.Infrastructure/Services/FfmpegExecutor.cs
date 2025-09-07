using Ffmpeg.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Services
{
    public class FFmpegExecutor
    {
        private readonly string _ffmpegPath;
        private readonly bool _logOutput;
        private readonly ILogger _logger;

        public FFmpegExecutor(string ffmpegPath, bool logOutput = false, ILogger logger = null)
        {
            _ffmpegPath = !string.IsNullOrEmpty(ffmpegPath)
                ? ffmpegPath
                : throw new ArgumentNullException(nameof(ffmpegPath), "FFmpeg path cannot be null or empty");
            _logOutput = logOutput;
            _logger = logger;
        }

        public async Task<(bool Success, string Output, string Error)> RunCommandAsync(string arguments)
        {
            if (_logOutput && _logger != null)
            {
                _logger.LogInformation($"Executing FFmpeg command: {_ffmpegPath} {arguments}");
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    outputBuilder.AppendLine(args.Data);
                    if (_logOutput && _logger != null)
                    {
                        _logger.LogDebug($"FFmpeg Output: {args.Data}");
                    }
                }
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    errorBuilder.AppendLine(args.Data);
                    if (_logOutput && _logger != null)
                    {
                        _logger.LogDebug($"FFmpeg Error: {args.Data}");
                    }
                }
            };

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                await process.WaitForExitAsync();

                return (
                    process.ExitCode == 0,
                    outputBuilder.ToString(),
                    errorBuilder.ToString()
                );
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Error executing FFmpeg command");
                }
                return (false, string.Empty, ex.ToString());
            }
        }
    }

}
