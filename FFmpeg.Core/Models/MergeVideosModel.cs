using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public enum MergeDirection
    {
        Horizontal,
        Vertical
    }
    public class MergeVideosModel
    {
        public string InputFile1 { get; set; }
        public string InputFile2 { get; set; }
        public string OutputFile { get; set; }
        public MergeDirection Direction { get; set; } = MergeDirection.Horizontal; // Default to horizontal
    }
}