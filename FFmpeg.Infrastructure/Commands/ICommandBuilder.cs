using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public interface ICommandBuilder
    {
        ICommandBuilder SetInput(string fileName);
        //ICommandBuilder SetInput(string fileName, bool isFullPath);
        ICommandBuilder AddFilterComplex(string filterExpression);
        ICommandBuilder SetOverlay(int x, int y, int? inputIndex = null, int? overlayIndex = null);
        ICommandBuilder SetOutput(string fileName, bool isFrameOutput = false, int frameCount = 1);
        //ICommandBuilder SetOutput(string fileName, bool isFullPath, bool isFrameOutput = false, int frameCount = 1);
        ICommandBuilder AddOption(string option);
        ICommandBuilder SetVideoCodec(string codec);
        ICommandBuilder SetVideoQuality(int crf);
        ICommandBuilder SetAudioCodec(string codec);
        ICommandBuilder SetFrameRate(int frameRate);
        ICommandBuilder SetScale(int width, int height);
        string Build();
    }
}
