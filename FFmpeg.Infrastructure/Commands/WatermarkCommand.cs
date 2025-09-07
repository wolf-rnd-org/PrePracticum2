using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ffmpeg.Command.Commands
{
    public class WatermarkCommand : BaseCommand, ICommand<WatermarkModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public WatermarkCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(WatermarkModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .SetInput(model.WatermarkFile)
                .SetOverlay(model.XPosition, model.YPosition)
                .AddOption($"-map 0:a?")
                .AddOption($"-c:a copy");

            if (model.IsVideo)
            {
                CommandBuilder
                    .SetVideoCodec(model.VideoCodec);
            }

            CommandBuilder.SetOutput(model.OutputFile, model.IsVideo ? false : true);

            return await RunAsync();
        }
    }
}
