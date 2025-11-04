using Foundation;

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class AppFileSystem
    {
        private readonly Lazy<string> _platformCacheDirectory = new(valueFactory: () =>
        {
            string path = Path.Combine(GetDirectory(NSSearchPathDirectory.CachesDirectory), NSBundle.MainBundle.BundleIdentifier);
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        });

        private readonly Lazy<string> _platformAppDataDirectory = new(valueFactory: () =>
        {
            string path = Path.Combine(GetDirectory(NSSearchPathDirectory.ApplicationSupportDirectory), NSBundle.MainBundle.BundleIdentifier);
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        });

        private static string GetDirectory(NSSearchPathDirectory directory)
        {
            var dirs = NSSearchPath.GetDirectories(directory, NSSearchPathDomain.User);
            if (dirs == null || dirs.Length == 0)
            {
                // this should never happen...
                return null;
            }
            return dirs[0];
        }
    }
}
