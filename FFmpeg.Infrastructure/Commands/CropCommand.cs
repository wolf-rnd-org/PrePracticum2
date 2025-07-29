using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class CropCommand : ICommand<CropRequest>
    {
        private readonly string _ffmpegPath;

        public CropCommand(string ffmpegPath = "ffmpeg")
        {
            _ffmpegPath = ffmpegPath;
        }

        public async Task<CommandResult> ExecuteAsync(CropRequest request)
        {
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
