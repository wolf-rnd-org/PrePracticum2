using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class MergeVideosModel
    {
        public string InputFile1 { get; set; }
        public string InputFile2 { get; set; }
        public string OutputFile { get; set; }
        public bool IsVertical { get; set; } = false;
    }
}
