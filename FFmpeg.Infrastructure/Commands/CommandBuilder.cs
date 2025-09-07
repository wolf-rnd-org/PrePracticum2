using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class CommandBuilder : ICommandBuilder
    {
        private readonly List<string> _inputs = new();
        private readonly List<string> _filters = new();
        private readonly List<string> _outputs = new();
        private readonly List<string> _options = new();
        private readonly IConfiguration _configuration;

        public CommandBuilder(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        private string GetPath()
        {
            var configPath = _configuration["FFmpeg:Path"];
            if (string.IsNullOrEmpty(configPath))
            {
                throw new InvalidOperationException("FFmpeg path is not configured in appsettings.json");
            }
            return Environment.ExpandEnvironmentVariables(configPath);
        }

        public ICommandBuilder SetInput(string fileName)
        {
            return SetInput(fileName, false);
        }

        public ICommandBuilder SetInput(string fileName, bool isFullPath)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            string inputPath = isFullPath
                ? fileName
                : Path.Combine(GetPath(), "Input", fileName);

            _inputs.Add($"-i \"{inputPath}\"");
            return this;
        }

        public ICommandBuilder AddFilterComplex(string filterExpression)
        {
            if (!string.IsNullOrEmpty(filterExpression))
            {
                _filters.Add(filterExpression);
            }
            return this;
        }

        public ICommandBuilder SetOverlay(int x, int y, int? inputIndex = null, int? overlayIndex = null)
        {
            string mainInput = inputIndex.HasValue ? $"[{inputIndex.Value}:v]" : "[0:v]";
            string overlayInput = overlayIndex.HasValue ? $"[{overlayIndex.Value}:v]" : "[1:v]";

            AddFilterComplex($"{mainInput}{overlayInput}overlay={x}:{y}[out]");
            return this;
        }

        public ICommandBuilder SetOutput(string fileName, bool isFrameOutput = false, int frameCount = 1)
        {
            return SetOutput(fileName, false, isFrameOutput, frameCount);
        }

        public ICommandBuilder SetOutput(string fileName, bool isFullPath, bool isFrameOutput = false, int frameCount = 1)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            string outputPath = isFullPath
                ? fileName
                : Path.Combine(GetPath(), "Output", fileName);

            string frameOption = isFrameOutput ? $"-frames:v {frameCount} " : "";
            _outputs.Add($"{frameOption}\"{outputPath}\"");
            return this;
        }

        public ICommandBuilder AddOption(string option)
        {
            if (!string.IsNullOrEmpty(option))
            {
                _options.Add(option);
            }
            return this;
        }

        public ICommandBuilder SetVideoCodec(string codec)
        {
            if (!string.IsNullOrEmpty(codec))
            {
                _options.Add($"-c:v {codec}");
            }
            return this;
        }

        public ICommandBuilder SetVideoQuality(int crf)
        {
            _options.Add($"-crf {crf}");
            return this;
        }

        public ICommandBuilder SetAudioCodec(string codec)
        {
            if (!string.IsNullOrEmpty(codec))
            {
                _options.Add($"-c:a {codec}");
            }
            return this;
        }

        public ICommandBuilder SetFrameRate(int frameRate)
        {
            _options.Add($"-r {frameRate}");
            return this;
        }

        public ICommandBuilder SetScale(int width, int height)
        {
            AddFilterComplex($"scale={width}:{height}");
            return this;
        }

        public string Build()
        {
            var command = new List<string>();

            // Add inputs
            command.AddRange(_inputs);

            // Add filter complex if any
            if (_filters.Count > 0)
            {
                command.Add($"-filter_complex \"{string.Join(';', _filters)}\"");
                command.Add("-map \"[out]\"");
            }

            // Add options
            command.AddRange(_options);

            // Add outputs
            command.AddRange(_outputs);

            return string.Join(" ", command);
        }
    }
}
