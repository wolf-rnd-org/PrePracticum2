namespace FFmpeg.API.DTOs
{
    public class BorderDto
    {
        public IFormFile VideoFile { get; set; }
        public string FrameColor { get; set; } = "black";
    }
}
