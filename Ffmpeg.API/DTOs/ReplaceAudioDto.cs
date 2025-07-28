namespace FFmpeg.API.DTOs
{
    public class ReplaceAudioDto
    {
        public IFormFile VideoFile { get; set; } = null!;
        public IFormFile NewAudioFile { get; set; } = null!;
    }
}