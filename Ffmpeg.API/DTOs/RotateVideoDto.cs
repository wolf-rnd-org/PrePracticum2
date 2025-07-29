namespace FFmpeg.API.DTOs
{
    public class RotateVideoDto
    {
        public IFormFile VideoFile { get; set; }
        public int Angle { get; set; } // לדוגמה: 90, 180, 270
    }
}