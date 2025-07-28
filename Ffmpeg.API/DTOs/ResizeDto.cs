namespace FFmpeg.API.DTOs
{
    public class ResizeDto
    {
        public IFormFile InputFile { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string OutputFile { get; set; }
    }
}
