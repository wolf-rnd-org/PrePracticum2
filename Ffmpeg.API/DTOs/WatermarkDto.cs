namespace FFmpeg.API.DTOs
{
    public class WatermarkDto
    {
        public IFormFile VideoFile { get; set; }
        public IFormFile WatermarkFile { get; set; }
        public int XPosition { get; set; } = 10;
        public int YPosition { get; set; } = 10;
    }
}
