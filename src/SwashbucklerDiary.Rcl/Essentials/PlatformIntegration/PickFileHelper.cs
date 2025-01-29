namespace SwashbucklerDiary.Rcl.Essentials
{
    public static partial class PlatformIntegrationHelper
    {
        public static readonly string[] ImageFileExtensions = [".jpg", ".jpeg", ".png", ".gif", ".svg", ".webp", ".jfif"];

        public static readonly string[] ImageMimeTypes = ["image/jpeg", "image/png", "image/gif", "image/svg+xml", "image/webp"];

        public static readonly string[] AudioFileExtensions = [".mp3", ".wav", ".m4a", ".ogg", ".aac", ".flac"];

        public static readonly string[] AudioMimeTypes = ["audio/mpeg", "audio/wav", "audio/mp4", "audio/ogg", "audio/aac", "audio/flac"];

        public static readonly string[] VideoFileExtensions = [".mp4", ".m4v", ".mpg", ".mpeg", ".mp2", ".mov", ".avi", ".mkv", ".flv", ".gifv", ".qt"];

        public static readonly string[] VideoMimeTypes = ["video/mp4", "video/mpeg", "video/quicktime", "video/x-msvideo", "video/x-matroska", "video/x-flv"];

        public static bool ValidFileExtensions(string? filePath, string[] fileExtensions)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            string extension = Path.GetExtension(filePath).ToLower();
            if (fileExtensions.Contains(extension))
            {
                return true;
            }

            return false;
        }
    }
}
