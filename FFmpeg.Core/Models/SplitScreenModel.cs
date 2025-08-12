using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class SplitScreenModel
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public int DuplicateCount { get; set; } = 2; // Default to duplicate twice
        public string VideoCodec { get; set; } = "libx264"; // Optional
    }
}
