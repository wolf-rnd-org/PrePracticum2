using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class RotateVideoCommand : BaseCommand, ICommand<RotateVideoModel>
    {
        public RotateVideoCommand(FFmpegExecutor executor, ICommandBuilder builder)
            : base(executor)
        {
            CommandBuilder = builder;
        }

        public async Task<CommandResult> ExecuteAsync(RotateVideoModel request)
        {
            string radians = $"{request.Angle}*PI/180";

            CommandBuilder
                .SetInput(request.InputPath)
                .AddFilterComplex($"rotate={radians}")
                .SetOutput(request.OutputPath);

            return await RunAsync();
        }
    }
}
