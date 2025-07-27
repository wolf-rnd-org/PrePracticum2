using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace Ffmpeg.Command.Commands
{
    public class ColorFilterCommand : BaseCommand, ICommand<ColorFilterModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public ColorFilterCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(ColorFilterModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption("-vf hue=s=0")        
                .AddOption("-map 0:a?")          
                .AddOption("-c:a copy");       

            if (model.IsVideo)
            {
                CommandBuilder.SetVideoCodec(model.VideoCodec); 
            }

            CommandBuilder.SetOutput(model.OutputFile, model.IsVideo ? false : true); 

            return await RunAsync(); 
        }
    }
}
