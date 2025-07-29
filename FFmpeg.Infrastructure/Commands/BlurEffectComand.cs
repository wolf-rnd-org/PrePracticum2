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
    public class BlurEffectComand(FFmpegExecutor executor, ICommandBuilder commandBuilder) : BaseCommand(executor), ICommand<BlurEffectModel>
    {
        private readonly ICommandBuilder _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        public Task<CommandResult> ExecuteAsync(BlurEffectModel model)
        {
            CommandBuilder = _commandBuilder.SetInput(model.VideoName)
                .SetOutput(model.OutputName)
                 .AddOption($"-vf \"gblur=sigma={model.Sigma}\"");
            return RunAsync();
        }
    }
}