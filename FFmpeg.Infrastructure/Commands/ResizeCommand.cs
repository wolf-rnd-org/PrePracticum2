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
    public class ResizeCommand : BaseCommand, ICommand<ResizeModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public ResizeCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(ResizeModel model)
        {
            string scaleFilter = $"scale={model.Width}:{model.Height}";

            if (model.Width <= 0 || model.Height <= 0)
            {
                throw new ArgumentException("Width and Height must be greater than zero.");
            }
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-vf {scaleFilter}")
                .SetVideoCodec(model.VideoCodec)
                .SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}
