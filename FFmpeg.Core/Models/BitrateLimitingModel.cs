using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class BitrateLimitingModel
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public string Bitrate { get; set; } = "1M"; // Default bitrate of 1 Mbps
        public bool IsVideo { get; set; } = true;
        public string VideoCodec { get; set; } = "libx264";
    }
}