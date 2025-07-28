namespace FFmpeg.API.DTOs
{
    public class SplitScreenDto
    {
        public IFormFile VideoFile { get; set; }
        public int DuplicateCount { get; set; } = 2;
    }
}
