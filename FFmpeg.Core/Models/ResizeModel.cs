using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class ResizeModel
    {

        public string InputFile { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string OutputFile { get; set; }
        public string VideoCodec { get; set; } = "libx264";

    }
}
