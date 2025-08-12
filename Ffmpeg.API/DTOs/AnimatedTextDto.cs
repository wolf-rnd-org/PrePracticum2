namespace FFmpeg.API.DTOs
{
    public class AnimatedTextDto
    {
        public IFormFile VideoFile { get; set; }
        public string TextContent { get; set; }
        public string Color { get; set; }
        public int FontSize { get; set; }
        public string OutputName { get; set; }
    }
}
