using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;


namespace FFmpeg.Infrastructure.Commands
{
    public class CutSectionCommand : BaseCommand, ICommand<CutSectionModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public CutSectionCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(CutSectionModel model)
        {
        
            var commandBuilder = _commandBuilder
               .SetInput(model.InputFilePath)
               .AddOption($"-ss {model.StartTime}")
               .AddOption($"-to {model.EndTime}")
               .AddOption("-c:v libx264")
               .AddOption("-c:a aac")
               .SetOutput(model.OutputFilePath);

            CommandBuilder = commandBuilder;

            return await RunAsync();
        }
    }
}
