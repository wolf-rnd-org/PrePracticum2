using Ffmpeg.Command;
using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FFmpeg.Infrastructure.Services
{
    public interface IFFmpegServiceFactory
    {
        ICommand<WatermarkModel> CreateWatermarkCommand();
        ICommand<ThumbnailModel> CreateThumbnailCommand();
        ICommand<BlurEffectModel> CreateBlurEffectCommand();
        ICommand<ReplaceAudioModel> CreateReplaceAudioCommand();
        ICommand<TimestampModel> CreateTimestampCommand();
        ICommand<MergeVideosModel> CreateMergeVideosCommand();
        ICommand<ConvertAudioModel> CreateConvertAudioCommand();
        ICommand<AnimatedTextModel> CreateAnimatedTextCommand();
        ICommand<GreenScreenModel> CreateGreenScreenCommand();
        ICommand<ColorFilterModel> CreateColorFilterCommand(); 
        ICommand<AudioMixModel> CreateMixAudioCommand();
        ICommand<ReverseVideoModel> ReverseVideoCommand();
        ICommand<ResizeModel> CreateResizeCommand();
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
        public ICommand<ThumbnailModel> CreateThumbnailCommand()
        {
            return new ThumbnailCommand(_executor, _commandBuilder);
        }
        public ICommand<BlurEffectModel> CreateBlurEffectCommand()
        {
            return new BlurEffectComand(_executor, _commandBuilder);
        }
        public ICommand<ReplaceAudioModel> CreateReplaceAudioCommand()
        {
            return new ReplaceAudioCommand(_executor, _commandBuilder);
        }
        public ICommand<TimestampModel> CreateTimestampCommand()
        {
            return new TimestampCommand(_executor, _commandBuilder);
        }
        public ICommand<MergeVideosModel> CreateMergeVideosCommand()
        {
            return new MergeVideosCommand(_executor, _commandBuilder);
        }
        public ICommand<ConvertAudioModel> CreateConvertAudioCommand()
        {
            return new ConvertAudioCommand(_executor, _commandBuilder);
        }
        public ICommand<AnimatedTextModel> CreateAnimatedTextCommand()
        {
            return new AnimatedTextCommand(_executor, _commandBuilder);
        }
        public ICommand<GreenScreenModel> CreateGreenScreenCommand()
        {
            return new GreenScreenReplacerCommand(_executor, _commandBuilder);
        }
        public ICommand<ColorFilterModel> CreateColorFilterCommand()
        {
            return new ColorFilterCommand(_executor, _commandBuilder);
        }
        public ICommand<AudioMixModel> CreateMixAudioCommand()
        {
            return new MixAudioCommand(_executor, _commandBuilder, new Logger());
        }
        public ICommand<ReverseVideoModel> ReverseVideoCommand()
        {
            return new ReverseVideoCommand(_executor, _commandBuilder);
        }
        public ICommand<ResizeModel> CreateResizeCommand()
        {
            return new ResizeCommand(_executor, _commandBuilder);
        }
    }
}
