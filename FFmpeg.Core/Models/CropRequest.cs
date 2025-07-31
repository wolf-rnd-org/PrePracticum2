using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class CropRequest
    {
        public string InputPath { get; set; } = "";
        public string OutputPath { get; set; } = "";
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Width { get; set; } = 640;
        public int Height { get; set; } = 480;
        public int X { get; set; } = 100;
        public int Y { get; set; } = 100;
    }
}
