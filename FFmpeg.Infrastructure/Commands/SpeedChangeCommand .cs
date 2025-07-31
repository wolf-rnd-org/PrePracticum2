using Ffmpeg.Command.Commands;
using FFmpeg.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFmpeg.Core.Models;
using Ffmpeg.Command;

namespace FFmpeg.Infrastructure.Commands
{

    public class SpeedChangeCommand : BaseCommand, ICommand<SpeedChangeModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public SpeedChangeCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(SpeedChangeModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (model.SpeedFactor <= 0)
                throw new ArgumentOutOfRangeException(nameof(model.SpeedFactor), "Speed factor must be greater than zero.");

            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-vf \"setpts={1 / model.SpeedFactor}*PTS\"")
                .SetOutput(model.OutputFile);

            // Get the built command string
            string commandString = CommandBuilder.Build();

            // Try to log the command using FFmpegExecutor's logger if available
            var loggerField = typeof(FFmpegExecutor).GetField("_logger", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var logger = loggerField?.GetValue(typeof(BaseCommand)
                .GetField("_executor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(this)) as ILogger;

            logger?.LogInformation($"FFmpeg command: {commandString}");

            var result = await RunAsync();

            return result;
        }
    }
}
