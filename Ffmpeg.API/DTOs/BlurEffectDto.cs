using System.ComponentModel.DataAnnotations;

namespace FFmpeg.API.DTOs
{
    public class BlurEffectDto
    {
        public IFormFile VideoFile { get; set; }

        [Range(0.1, 100)]
        public double Sigma { get; set; } = 10;
    }
}
