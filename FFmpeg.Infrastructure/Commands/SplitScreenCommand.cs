using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;

namespace FFmpeg.Infrastructure.Commands
{
    public class SplitScreenCommand : BaseCommand, ICommand<SplitScreenModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public SplitScreenCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(SplitScreenModel model)
        {
            
            if (model.DuplicateCount != 2)
            {
                throw new NotSupportedException("Currently only duplication of 2 is supported using hstack.");
            }

            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddFilterComplex("[0:v]hstack=inputs=2[out]")
                .SetVideoCodec(model.VideoCodec)
                .SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}
