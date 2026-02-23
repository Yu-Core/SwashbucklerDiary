using Microsoft.Maui.ApplicationModel;

namespace Microsoft.Maui.Storage
{
    // from https://github.com/dotnet/maui/blob/8423d5c68930aa6863b1e02e3eaf31c37db6d513/src/Essentials/src/FileSystem/FileSystem.windows.cs
    partial class FileSystemImplementation : IFileSystem
    {
        private readonly Lazy<string> _platformCacheDirectory = new(valueFactory: () =>
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppSpecificPath, "Cache");

            if (!File.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        });

        private readonly Lazy<string> _platformAppDataDirectory = new(valueFactory: () =>
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppSpecificPath, "Data");

            if (!File.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        });

        static string CleanPath(string path) =>
            string.Join("_", path.Split(Path.GetInvalidFileNameChars()));

        static string AppSpecificPath =>
            Path.Combine(CleanPath(AppInfoImplementation.PublisherName), CleanPath(AppInfo.PackageName));

        string PlatformCacheDirectory => _platformCacheDirectory.Value;

        string PlatformAppDataDirectory => _platformAppDataDirectory.Value;

        Task<Stream> PlatformOpenAppPackageFileAsync(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));


            var file = FileSystemUtils.PlatformGetFullAppPackageFilePath(filename);
            return Task.FromResult((Stream)File.OpenRead(file));
        }

        Task<bool> PlatformAppPackageFileExistsAsync(string filename)
        {
            var file = FileSystemUtils.PlatformGetFullAppPackageFilePath(filename);
            return Task.FromResult(File.Exists(file));
        }
    }

}