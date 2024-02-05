using Microsoft.AspNetCore.Components.WebView.Maui;

namespace SwashbucklerDiary.Maui.BlazorWebView
{
    public partial class MauiBlazorWebViewHandler : BlazorWebViewHandler
    {
        private const string AppHostAddress = "0.0.0.0";
#if IOS || MACCATALYST
        public const string BaseUri = $"app://{AppHostAddress}/";
#else
        public const string BaseUri = $"https://{AppHostAddress}/";
#endif
        public static readonly Dictionary<string, string> AppFilePathMap = new()
        {
            { FileSystem.AppDataDirectory, "appdata" },
            { FileSystem.CacheDirectory, "cache" },
        };

        private static readonly string OtherFileMapPath = "file";

        //把真实的文件路径转化为url相对路径
        public static string FilePathToUrlRelativePath(string filePath)
        {
            foreach (var item in AppFilePathMap)
            {
                if (filePath.StartsWith(item.Key))
                {
                    return item.Value + filePath[item.Key.Length..].Replace(Path.DirectorySeparatorChar, '/');
                }
            }

            return OtherFileMapPath + "/" + Uri.EscapeDataString(filePath);
        }

        //把url相对路径转化为真实的文件路径
        public static string UrlRelativePathToFilePath(string urlRelativePath)
        {
            UrlRelativePathToFilePath(urlRelativePath, out string path);
            return path;
        }

        private static bool Intercept(string uri, out string path)
        {
            if (!uri.StartsWith(BaseUri))
            {
                path = string.Empty;
                return false;
            }

            uri = new Uri(uri).GetLeftPart(UriPartial.Path);
            var urlRelativePath = uri[BaseUri.Length..];
            return UrlRelativePathToFilePath(urlRelativePath, out path);
        }

        private static bool UrlRelativePathToFilePath(string urlRelativePath, out string path)
        {
            if (string.IsNullOrEmpty(urlRelativePath))
            {
                path = string.Empty;
                return false;
            }

            urlRelativePath = Uri.UnescapeDataString(urlRelativePath);

            foreach (var item in AppFilePathMap)
            {
                if (urlRelativePath.StartsWith(item.Value + '/'))
                {
                    string urlRelativePathSub = urlRelativePath[(item.Value.Length + 1)..];
                    path = Path.Combine(item.Key, urlRelativePathSub.Replace('/', Path.DirectorySeparatorChar));
                    if (File.Exists(path))
                    {
                        return true;
                    }
                }
            }

            if (urlRelativePath.StartsWith(OtherFileMapPath + '/'))
            {
                string urlRelativePathSub = urlRelativePath[(OtherFileMapPath.Length + 1)..];
                path = urlRelativePathSub.Replace('/', Path.DirectorySeparatorChar);
                if (File.Exists(path))
                {
                    return true;
                }
            }

            path = string.Empty;
            return false;
        }
    }
}
