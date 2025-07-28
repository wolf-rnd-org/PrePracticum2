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
    public class MergeVideosCommand : BaseCommand, ICommand<MergeVideosModel>
    {
        private readonly ICommandBuilder _commandBuilder;
        public MergeVideosCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }
        public async Task<CommandResult> ExecuteAsync(MergeVideosModel model)
        {
            string filterComplex = model.Direction == MergeDirection.Horizontal
                ? "[0:v]scale=-1:720[vid1];[1:v]scale=-1:720[vid2];[vid1][vid2]hstack=inputs=2[out]"
                : "[0:v]scale=1280:-1[vid1];[1:v]scale=1280:-1[vid2];[vid1][vid2]vstack=inputs=2[out]";
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile1)
                .SetInput(model.InputFile2)
                .AddFilterComplex(filterComplex)
                .AddOption("-an")
                .SetOutput(model.OutputFile);
            return await RunAsync();
        }
    }
}
