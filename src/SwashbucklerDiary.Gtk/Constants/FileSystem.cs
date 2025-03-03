namespace SwashbucklerDiary.Gtk
{
    public static class FileSystem
    {
        private static readonly Lazy<string> _platformCacheDirectory = new(valueFactory: () =>
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppSpecificPath, "Cache");

            if (!File.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        });

        private static readonly Lazy<string> _platformAppDataDirectory = new(valueFactory: () =>
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppSpecificPath, "Data");

            if (!File.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        });

        public static string AppDataDirectory => _platformAppDataDirectory.Value;

        public static string CacheDirectory => _platformCacheDirectory.Value;

        static string AppSpecificPath => AppInfo.PackageName;
    }
}

