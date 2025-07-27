using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;
using System.Threading.Tasks;

namespace Ffmpeg.Command.Commands
{
    public class MergeVideosCommand : BaseCommand, ICommand<MergeVideosModel>
    {
        private readonly ICommandBuilder _builder;

        public MergeVideosCommand(FFmpegExecutor executor, ICommandBuilder builder)
            : base(executor)
        {
            _builder = builder;
        }

        public async Task<CommandResult> ExecuteAsync(MergeVideosModel model)
        {
            string layout = model.IsVertical ? "vstack=inputs=2" : "hstack=inputs=2";

            CommandBuilder = _builder
                .SetInput(model.InputFile1)
                .SetInput(model.InputFile2)
                .AddFilterComplex($"[0:v][1:v]{layout}[out]")
                .AddOption("-map \"[out]\"")
                .SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}
