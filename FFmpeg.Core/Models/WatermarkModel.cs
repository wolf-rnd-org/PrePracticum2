using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class WatermarkModel
    {
        public string InputFile { get; set; }
        public string WatermarkFile { get; set; }
        public string OutputFile { get; set; }
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        public bool IsVideo { get; set; }
        public string VideoCodec { get; set; } = "libx264"; // Default codec
    }
}
