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
    class ThumbnailCommand : BaseCommand, ICommand<ThumbnailModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public ThumbnailCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        // Creates a thumbnail from a video at 5 seconds
        public async Task<CommandResult> ExecuteAsync(ThumbnailModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.VideoFile)
                .AddOption("-ss 00:00:05")
                .AddOption("-vframes 1")
                .SetOutput(model.OutputFile, true, 1);

            return await RunAsync();
        }
    }
}
