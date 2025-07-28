using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;
using Ffmpeg.Command;
using System;
using System.Threading.Tasks;

namespace Ffmpeg.Command.Commands
{
    public class MixAudioCommand : BaseCommand, ICommand<AudioMixModel>
    {
        private readonly ICommandBuilder _commandBuilder;
        private readonly ILogger _logger;

        public MixAudioCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder, ILogger logger) : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CommandResult> ExecuteAsync(AudioMixModel model)
        {
            CommandBuilder = _commandBuilder
              .SetInput(model.InputFile1)
              .SetInput(model.InputFile2)
              .AddFilterComplex("[0:a][1:a]amix=inputs=2:duration=longest[aout]")
              .AddOption("-map [aout]")
              .AddOption("-c:a libmp3lame")
              .SetOutput(model.OutputFile);

            _logger.LogInformation($"FFmpeg command: {CommandBuilder.Build()}");

            return await RunAsync();
        }
    }
}