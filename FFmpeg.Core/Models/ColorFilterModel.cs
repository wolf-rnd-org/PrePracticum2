using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class ColorFilterModel
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public bool IsVideo { get; set; } = true;
        public string VideoCodec { get; set; } = "libx264";
    }
}
