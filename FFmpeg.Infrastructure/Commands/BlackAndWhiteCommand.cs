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
    public class BlackAndWhiteCommand : BaseCommand, ICommand<BlackAndWhiteModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public BlackAndWhiteCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(BlackAndWhiteModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddFilterComplex("hue=s=0")
                .SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}
