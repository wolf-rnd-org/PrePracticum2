using Microsoft.AspNetCore.Http;
using FFmpeg.Core.Models; // במקום ההגדרה הכפולה


namespace FFmpeg.API.DTOs
{
    public class MergeVideosDto
    {
        public IFormFile InputFile1 { get; set; }
        public IFormFile InputFile2 { get; set; }
        public MergeDirection Direction { get; set; } = MergeDirection.Horizontal;
    }
}
