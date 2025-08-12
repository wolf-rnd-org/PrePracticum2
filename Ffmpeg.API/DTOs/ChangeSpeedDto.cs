namespace FFmpeg.API.DTOs
{
    public class ChangeSpeedDto
    {
        public IFormFile VideoFile { get; set; } // קובץ הווידאו
        public double SpeedFactor { get; set; } // מהירות חדשה (למשל, 0.5 להאטה, 2.0 להאצה)
        public string OutputFileName { get; set; } // שם הקובץ לפלט
    }
}
