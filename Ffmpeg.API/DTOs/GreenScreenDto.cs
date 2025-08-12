namespace FFmpeg.API.DTOs
{
    public class GreenScreenDto
    {
        public IFormFile InputFile { get; set; }

        public IFormFile BackgroundFile { get; set; }

        public double? Similarity { get; set; } = 0.1;

        public double? Blend { get; set; } = 0.1;

        public string? ChromaColor { get; set; } = "0x00FF00";
    }
}
