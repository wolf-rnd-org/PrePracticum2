namespace FFmpeg.API.DTOs
{
    public class GifDto
    {
        public IFormFile VideoFile { get; set; }
        public int Fps { get; set; } = 10;
        public int Width { get; set; } = 320;
    }
}
