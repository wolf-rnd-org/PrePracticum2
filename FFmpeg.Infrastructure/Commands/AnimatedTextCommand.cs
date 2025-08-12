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
    public class AnimatedTextCommand : BaseCommand, ICommand<AnimatedTextModel>
    {
        private readonly ICommandBuilder _commandBuilder;
        public AnimatedTextCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }
        public async Task<CommandResult> ExecuteAsync(AnimatedTextModel model)
        {
            string drawTextFilter = $"drawtext=text='{model.Text}':x=100:y=50:fontsize={model.FontSize}:fontcolor={model.Color}";
            CommandBuilder = _commandBuilder
                .SetInput(model.VideoPath)
                .AddOption($"-vf \"{drawTextFilter}\"")
                .SetOutput(model.OutputPath);

            return await RunAsync();
        }
    }
}
