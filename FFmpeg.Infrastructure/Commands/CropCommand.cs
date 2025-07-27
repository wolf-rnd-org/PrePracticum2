using Ffmpeg.Command.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class CropCommand : ICommand
    {
        private readonly string _ffmpegPath;

        public CropCommand(string ffmpegPath = "ffmpeg")
        {
            _ffmpegPath = ffmpegPath;
        }

        public async Task<CommandResult> RunAsync(object parameter)
        {
            if (parameter is not CropRequest request)
                return CommandResult.Failure("Invalid request type");

            var duration = request.EndTime - request.StartTime;

            string arguments = $"-i \"{request.InputPath}\" -ss {request.StartTime} -t {duration} -vf \"crop={request.Width}:{request.Height}:{request.X}:{request.Y}\" -c:a copy \"{request.OutputPath}\"";

            var startInfo = new ProcessStartInfo
            {
                FileName = _ffmpegPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = startInfo };
            process.Start();
            string error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            return process.ExitCode == 0
                ? CommandResult.Success()
                : CommandResult.Failure(error);
        }
    }
}
