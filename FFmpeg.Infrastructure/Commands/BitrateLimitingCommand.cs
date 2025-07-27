using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;

namespace FFmpeg.Infrastructure.Commands
{
    public class BitrateLimitingCommand : BaseCommand, ICommand<BitrateLimitingModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public BitrateLimitingCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(BitrateLimitingModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-b:v {model.Bitrate}")
                .SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}