using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class GreenScreenReplacerCommand : BaseCommand, ICommand<GreenScreenModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public GreenScreenReplacerCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder) : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(GreenScreenModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .SetInput(model.BackgroundFile)
                .AddOption("-filter_complex")
                .AddOption($"[0:v]chromakey={model.ChromaColor}:{model.Similarity}:{model.Blend}[ckout];[1:v][ckout]overlay[out]")
                .AddOption("-map")
                .AddOption("[out]")
                .AddOption("-map")
                .AddOption("0:a?")
                .AddOption("-c:a")
                .AddOption("copy");

            if (!string.IsNullOrWhiteSpace(model.VideoCodec))
            {
                CommandBuilder.SetVideoCodec(model.VideoCodec);
            }

            CommandBuilder.SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}
