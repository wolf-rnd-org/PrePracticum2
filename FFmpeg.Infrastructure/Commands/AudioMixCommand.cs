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
    public class AudioMixCommand : BaseCommand, ICommand<AudioMixModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public AudioMixCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(AudioMixModel model)
        {
            if (model.InputFiles == null || model.InputsCount < 2)
            {
                return new CommandResult
                {
                    IsSuccess = false,
                    ErrorMessage = "At least two input files are required for audio mixing."
                };
            }
            // Add all input files
            foreach (var inputFile in model.InputFiles)
            {
                _commandBuilder.SetInput(inputFile);
            }
            // Build the amix filter
            string filter = $"amix=inputs={model.InputsCount}";
            _commandBuilder.AddFilterComplex(filter);
            // Set output file
            _commandBuilder.SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}
