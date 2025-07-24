//using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using FFmpeg.Infrastructure.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ffmpeg.Command.Commands;

namespace Ffmpeg.Command.Commands
{
    public class ReverseVideoCommand: BaseCommand, ICommand<ReverseVideoModel>
    {
        private readonly ICommandBuilder _commandBuilder;
        public ReverseVideoCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            :base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(ReverseVideoModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption("-vf reverse")
                .AddOption("-an");

            CommandBuilder.SetOutput(model.OutputFile, false);

            return await RunAsync();
        }
    }
}
