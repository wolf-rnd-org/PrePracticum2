using FFmpeg.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{

    public class AddAnimatedTextCommand
    {
        private readonly string _ffmpegPath;

        public AddAnimatedTextCommand(string ffmpegPath)
        {
            _ffmpegPath = ffmpegPath;
        }

        public void Execute(AnimatedTextModel model)
        {
            string arguments = $"-i \"{model.VideoPath}\" -vf \"drawtext=text='{model.Text}':x=100:y=50:fontsize={model.FontSize}:fontcolor={model.Color}\" \"{model.OutputPath}\"";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = _ffmpegPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();
                process.WaitForExit();
            }
        }
    }
}
