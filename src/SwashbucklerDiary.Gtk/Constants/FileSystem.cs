using SwashbucklerDiary.Gtk.Essentials;

namespace SwashbucklerDiary.Gtk
{
    public static class FileSystem
    {
        public static string AppDataDirectory => PlatformAppDataDirectory;

        public static string CacheDirectory => PlatformCacheDirectory;

        static string CleanPath(string path) =>
            string.Join("_", path.Split(Path.GetInvalidFileNameChars()));

        static string AppSpecificPath =>
            Path.Combine(CleanPath(PlatformIntegration.PublisherName), CleanPath(PlatformIntegration.PackageName));

        static string PlatformCacheDirectory
           => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppSpecificPath, "Cache");

        static string PlatformAppDataDirectory
            => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppSpecificPath, "Data");
    }
}

