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
    public class ConvertVideoCommand : BaseCommand, ICommand<ConvertVideoModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public ConvertVideoCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(ConvertVideoModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .SetVideoCodec(model.VideoCodec)
                .SetAudioCodec(model.AudioCodec)
                .SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}
