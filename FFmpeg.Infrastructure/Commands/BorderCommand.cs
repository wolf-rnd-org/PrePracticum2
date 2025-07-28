using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ffmpeg.Command.Commands
{
    public class BorderCommand : BaseCommand, ICommand<BorderModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public BorderCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(BorderModel model)
        {
            // נוסיף 40 לפיקסלים לרוחב ולגובה, ומרכז המסגרת יהיה ב-20 פיקסלים מכל צד (כמו בדוגמה)
            // color לפי הצבע שהמשתמש בחר

            string vfFilter = $"pad=width=iw+40:height=ih+40:x=20:y=20:color={model.FrameColor}";

            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-vf \"{vfFilter}\"")  // פילטר pad להוספת המסגרת
                .AddOption("-c:a copy")            // העתקת השמע ללא שינוי
                .SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}

