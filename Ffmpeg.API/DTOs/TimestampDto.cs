namespace FFmpeg.API.DTOs
{
    public class TimestampDto
    {
        public IFormFile VideoFile { get; set; }
        public string OutputFile { get; set; }
        public int XPosition { get; set; } = 10;
        public int YPosition { get; set; } = 10;
        public int FontSize { get; set; } = 24;
        public string FontColor { get; set; } = "white";
    }
}
