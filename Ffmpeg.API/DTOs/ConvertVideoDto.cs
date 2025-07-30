using System.ComponentModel.DataAnnotations;

namespace FFmpeg.API.DTOs
{
    public class ConvertVideoDto
    {
        [Required]
        public IFormFile VideoFile { get; set; } = null!;
        [Required]
        public string OutputFileName { get; set; } = string.Empty;
        public string VideoCodec { get; set; } = "libx264";
        public string AudioCodec { get; set; } = "aac";
    }
}
