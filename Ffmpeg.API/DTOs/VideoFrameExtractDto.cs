using System.ComponentModel.DataAnnotations;

namespace FFmpeg.API.DTOs
{
    public class VideoFrameExtractDto
    {
        [Required]
        public IFormFile VideoFile { get; set; } = null!;

        [Required]
        public string TimePosition { get; set; } = "00:00:05"; // Format: HH:MM:SS

        [Required]
        public string OutputImageName { get; set; } = string.Empty; // Should include extension (.png, .jpg, etc.)
    }
}
