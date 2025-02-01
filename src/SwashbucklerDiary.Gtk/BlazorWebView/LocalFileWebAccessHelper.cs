using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Gtk.BlazorWebView
{
    public static partial class LocalFileWebAccessHelper
    {
        public static Dictionary<string, string> AppFilePathMap { get; } = new()
        {
            { FileSystem.AppDataDirectory, $"{AppFileSystem.AppDataVirtualDirectoryName}/" },
            { FileSystem.CacheDirectory, $"{AppFileSystem.CacheVirtualDirectoryName}/" },
        };

        private const string otherFileUrlPrefix = "file/";

        // 将真实的文件路径转化为 URL 相对路径
        public static string FilePathToUrlRelativePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return string.Empty;
            }

            foreach (var (directoryPath, urlPrefix) in AppFilePathMap)
            {
                if (filePath.StartsWith(directoryPath, StringComparison.OrdinalIgnoreCase))
                {
                    var relativeFilePath = filePath[(directoryPath.Length + 1)..];
                    return $"{urlPrefix}{relativeFilePath.Replace(Path.DirectorySeparatorChar, '/')}";
                }
            }

            return $"{otherFileUrlPrefix}{Uri.EscapeDataString(filePath)}";
        }

        // 将 URL 相对路径转化为真实的文件路径
        public static string UrlRelativePathToFilePath(string urlRelativePath)
        {
            if (string.IsNullOrWhiteSpace(urlRelativePath))
            {
                return string.Empty;
            }

            urlRelativePath = Uri.UnescapeDataString(urlRelativePath);

            foreach (var (directoryPath, urlPrefix) in AppFilePathMap)
            {
                if (urlRelativePath.StartsWith(urlPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    var relativePath = urlRelativePath[urlPrefix.Length..];
                    return Path.Combine(directoryPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
                }
            }

            if (urlRelativePath.StartsWith(otherFileUrlPrefix, StringComparison.OrdinalIgnoreCase))
            {
                var relativePath = urlRelativePath[otherFileUrlPrefix.Length..];
                return relativePath.Replace('/', Path.DirectorySeparatorChar);
            }

            return string.Empty;
        }
    }
}
