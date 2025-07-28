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
    public class ConvertAudioCommand : BaseCommand, ICommand<ConvertAudioModel>
    {
        private readonly ICommandBuilder _builder;

        public ConvertAudioCommand(FFmpegExecutor executor, ICommandBuilder builder)
            : base(executor)
        {
            _builder = builder;
        }

        public async Task<CommandResult> ExecuteAsync(ConvertAudioModel model)
        {
            CommandBuilder = _builder
                .SetInput(model.InputFile)
                .SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}
