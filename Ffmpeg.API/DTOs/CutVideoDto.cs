namespace FFmpeg.API.DTOs
{
    public class CutVideoDto
    {
        public IFormFile VideoFile { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string OutputFileName { get; set; }
    }
}
