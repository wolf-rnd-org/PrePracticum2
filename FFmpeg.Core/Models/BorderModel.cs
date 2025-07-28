using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class BorderModel
    {
        public string InputFile { get; set; }
        public string FrameColor { get; set; } = "black";
        public string OutputFile { get; set; }
    }
}