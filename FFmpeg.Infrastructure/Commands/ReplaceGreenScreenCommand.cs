using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class ReplaceGreenScreenCommand : BaseCommand, ICommand<ReplaceGreenScreen>
    {
        private readonly ICommandBuilder _commandBuilder;

        public ReplaceGreenScreenCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(ReplaceGreenScreen request)
        {
            _commandBuilder
                .SetInput(request.InputVideoName)
                .SetInput(request.BackgroundVideoName)
                .AddFilterComplex("[0:v]chromakey=0x00FF00:0.1:0.2[ckout];[1:v][ckout]overlay[out]")
                .SetOutput(request.OutputVideoName);

            return await RunAsync();
        }
    }
}
