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
    public class BrightnessContrastCommand : BaseCommand, ICommand<BrightnessContrastModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public BrightnessContrastCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(BrightnessContrastModel model)
        {
            string filter = $"eq=brightness={model.Brightness}:contrast={model.Contrast}";

            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddFilterComplex(filter)
                .SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}
