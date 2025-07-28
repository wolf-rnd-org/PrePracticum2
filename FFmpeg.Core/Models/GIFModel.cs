using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class GIFModel
    {
        public string InputFile { get; set; } = string.Empty; // הוספתי את הנכס החסר
        public string OutputGifName { get; set; } = string.Empty; // הוספתי את הנכס החסר
        public bool IsVideo { get; set; } = true;
        public string VideoCodec { get; set; } = "libx264";
    }
}