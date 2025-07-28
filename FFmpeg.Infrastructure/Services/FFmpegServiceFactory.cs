using Ffmpeg.Command;
using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Services
{
    public interface IFFmpegServiceFactory
    {
        ICommand<WatermarkModel> CreateWatermarkCommand();
        ICommand<TimestampModel> CreateTimestampCommand();
        ICommand<ConvertAudioModel> CreateConvertAudioCommand();
        ICommand<SpeedChangeModel> CreateChangeSpeedCommand(); // הוספת שיטה חדשה
    }

    public class FFmpegServiceFactory : IFFmpegServiceFactory
    {
        private readonly FFmpegExecutor _executor;
        private readonly ICommandBuilder _commandBuilder;

        public FFmpegServiceFactory(IConfiguration configuration, ILogger logger = null)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string ffmpegPath = Path.Combine(baseDirectory, "external", "ffmpeg.exe");

            bool logOutput = bool.TryParse(configuration["FFmpeg:LogOutput"], out bool log) && log;

            _executor = new FFmpegExecutor(ffmpegPath, logOutput, logger);
            _commandBuilder = new CommandBuilder(configuration);
        }

        public ICommand<WatermarkModel> CreateWatermarkCommand()
        {
            return new WatermarkCommand(_executor, _commandBuilder);
        }

        public ICommand<TimestampModel> CreateTimestampCommand()
        {
            return new TimestampCommand(_executor, _commandBuilder);
        }

        public ICommand<ConvertAudioModel> CreateConvertAudioCommand()
        {
            return new ConvertAudioCommand(_executor, _commandBuilder);
        }
        public ICommand<SpeedChangeModel> CreateChangeSpeedCommand() // מימוש השיטה החדשה
        {
            return new SpeedChangeCommand(_executor, _commandBuilder);
        }


    }
}
