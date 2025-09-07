namespace FFmpeg.API
{
    public static class Helper
    {
        private static string GetOutputExtension(string format)
        {
            return format?.ToLowerInvariant() switch
            {
                "mp4" => ".mp4",
                "webm" => ".webm",
                "mkv" => ".mkv",
                "avi" => ".avi",
                "mov" => ".mov",
                "gif" => ".gif",
                _ => ".mp4" // Default to MP4
            };
        }

        private static string GetVideoCodec(string format)
        {
            return format?.ToLowerInvariant() switch
            {
                "mp4" => "libx264",
                "webm" => "libvpx-vp9",
                "mkv" => "libx264",
                "avi" => "libx264",
                "mov" => "libx264",
                "gif" => "gif",
                _ => "libx264" // Default to H.264
            };
        }

        private static string GetAudioCodec(string format)
        {
            return format?.ToLowerInvariant() switch
            {
                "mp4" => "aac",
                "webm" => "libopus",
                "mkv" => "aac",
                "avi" => "aac",
                "mov" => "aac",
                "gif" => "copy", // GIF has no audio
                _ => "aac" // Default to AAC
            };
        }

        private static string GetContentType(string format)
        {
            return format?.ToLowerInvariant() switch
            {
                "mp4" => "video/mp4",
                "webm" => "video/webm",
                "mkv" => "video/x-matroska",
                "avi" => "video/x-msvideo",
                "mov" => "video/quicktime",
                "gif" => "image/gif",
                _ => "video/mp4" // Default to MP4
            };
        }
    }
}
