namespace FFmpeg.API.DTOs
{
    public class CropDto
    {
        public IFormFile VideoFile { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
