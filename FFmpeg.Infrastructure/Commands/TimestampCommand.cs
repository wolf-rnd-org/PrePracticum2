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
    public class TimestampCommand : BaseCommand, ICommand<TimestampModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public TimestampCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(TimestampModel model)
        {
            string drawtextFilter = $"[v:0]drawtext=text='%{{pts\\:hms}}':x={model.XPosition}:y={model.YPosition}:" +
                $"fontsize={model.FontSize}:fontcolor={model.FontColor}[out]";

            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddFilterComplex(drawtextFilter);
                //.AddOption($"-map 0:a?")
                //.AddOption($"-c:a copy");

            if (model.IsVideo)
            {
                CommandBuilder.SetVideoCodec(model.VideoCodec);
            }

            CommandBuilder.SetOutput(model.OutputFile, model.IsVideo ? false : true);

            return await RunAsync();
        }
    }

}
