namespace FFmpeg.API.DTOs
{
    public class BitrateLimitingDTO
    {
        public IFormFile VideoFile { get; set; }
        public IFormFile BitrateLimitingFile { get; set; }
        public string Bitrate { get; set; } = "1M"; // Default bitrate of 1 Mbps
    }
}
