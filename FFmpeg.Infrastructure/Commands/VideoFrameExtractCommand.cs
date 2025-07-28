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
    public class VideoFrameExtractCommand : BaseCommand, ICommand<VideoFrameExtractModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public VideoFrameExtractCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(VideoFrameExtractModel model)
        {
            // Validate time format (basic validation)
            if (string.IsNullOrEmpty(model.TimePosition))
            {
                throw new ArgumentException("Time position cannot be null or empty");
            }

            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-ss {model.TimePosition}")
                .AddOption("-vframes 1")
                .SetOutput(model.OutputFile, true, 1); // isFrameOutput = true, frameCount = 1

            return await RunAsync();
        }
    }
}
