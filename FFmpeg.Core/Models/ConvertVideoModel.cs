using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class ConvertVideoModel
    {
        [Required]
        public string InputFile { get; set; } = string.Empty;
        [Required]
        public string OutputFile { get; set; } = string.Empty;
        public string VideoCodec { get; set; } = "libx264";
        public string AudioCodec { get; set; } = "aac";
    }
}
