namespace FFmpeg.API.DTOs
{
    public class ThumbnailDto
    {
        public IFormFile VideoFile { get; set; }
        public string OutputFileName { get; set; }
    }
}
