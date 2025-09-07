using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ffmpeg.Command.Commands
{
    public abstract class BaseCommand
    {
        private readonly FFmpegExecutor _executor;
        protected ICommandBuilder CommandBuilder { get; set; }

        protected BaseCommand(FFmpegExecutor executor)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        protected async Task<CommandResult> RunAsync()
        {
            string arguments = CommandBuilder.Build();
            var (success, output, error) = await _executor.RunCommandAsync(arguments);

            return new CommandResult
            {
                IsSuccess = success,
                ErrorMessage = success ? string.Empty : $"Command failed: {error}",
                CommandExecuted = arguments,
                OutputLog = success ? output : error
            };
        }
    }
}
