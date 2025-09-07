using Ffmpeg.Command;
using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;

namespace FFmpeg.Infrastructure.Commands
{
    public class VideoCompressionCommand : BaseCommand, ICommand<VideoCompressionModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public VideoCompressionCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(VideoCompressionModel model)
        {
            if (string.IsNullOrEmpty(model.InputFile) || string.IsNullOrEmpty(model.OutputFile))
            {
                return new CommandResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Both input and output file names must be provided."
                };
            }

            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .SetVideoCodec("libx264")
                .AddOption("-crf 28")
                .SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}
