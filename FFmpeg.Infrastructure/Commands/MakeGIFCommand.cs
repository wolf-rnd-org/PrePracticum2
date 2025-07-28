using Ffmpeg.Command.Commands;
using FFmpeg.Infrastructure.Services;
using FFmpeg.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class MakeGIFCommand : BaseCommand, ICommand<GIFModel>
    {
        private readonly ICommandBuilder _commandBuilder;
        public WatermarkCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(GIFModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption("-vf \"fps=10,scale=320:-1\"")
                .SetOutput(model.OutputGifName);
            if (model.IsVideo)
            {
                CommandBuilder.SetVideoCodec(model.VideoCodec);
            }
            return await RunAsync();
        }

    }
}
