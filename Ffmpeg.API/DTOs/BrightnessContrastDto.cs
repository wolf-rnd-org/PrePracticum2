namespace FFmpeg.API.DTOs
{
    public class BrightnessContrastDto
    {
        public IFormFile VideoFile { get; set; }
        public float Brightness { get; set; }  // ערכים בין -1 ל־1
        public float Contrast { get; set; }    // ערכים בין 0 ל־2
    }
}
