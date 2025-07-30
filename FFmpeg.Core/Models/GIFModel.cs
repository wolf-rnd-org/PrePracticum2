using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class GIFModel
    {
        public string InputVideoName { get; set; }
        public string OutputVideoName { get; set; }
        public bool IsVideo { get; set; }
        public string VideoCodec { get; set; } = "libx264";
    }
}
