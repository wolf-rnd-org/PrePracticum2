using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Core.Models
{
    public class VideoFrameExtractModel
    {
        [Required]
        public string InputFile { get; set; } = string.Empty;

        [Required]
        public string OutputFile { get; set; } = string.Empty;

        [Required]
        public string TimePosition { get; set; } = "00:00:05"; // Default: 5 seconds

        public string ImageFormat { get; set; } = "png"; // Default format: PNG
    }
}
