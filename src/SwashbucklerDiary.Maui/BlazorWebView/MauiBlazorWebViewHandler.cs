using Microsoft.AspNetCore.Components.WebView.Maui;

namespace SwashbucklerDiary.Maui.BlazorWebView
{
    public partial class MauiBlazorWebViewHandler : BlazorWebViewHandler
    {
        public static string AppHostAddress { get; } = GetAppHostAddress();
#if IOS || MACCATALYST
        public static string BaseUri { get; } = $"app://{AppHostAddress}/";
#else
        public static string BaseUri { get; } = $"https://{AppHostAddress}/";
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
            if (string.IsNullOrEmpty(urlRelativePath))
            {
                return string.Empty;
            }

            urlRelativePath = Uri.UnescapeDataString(urlRelativePath);

            foreach (var item in AppFilePathMap)
            {
                if (urlRelativePath.StartsWith(item.Value + '/'))
                {
                    string urlRelativePathSub = urlRelativePath[(item.Value.Length + 1)..];
                    return Path.Combine(item.Key, urlRelativePathSub.Replace('/', Path.DirectorySeparatorChar));
                }
            }

            if (urlRelativePath.StartsWith(OtherFileMapPath + '/'))
            {
                string urlRelativePathSub = urlRelativePath[(OtherFileMapPath.Length + 1)..];
                return urlRelativePathSub.Replace('/', Path.DirectorySeparatorChar);
            }

            return string.Empty;
        }

        private static bool Intercept(string uri, out string filePath)
        {
            if (!uri.StartsWith(BaseUri))
            {
                filePath = string.Empty;
                return false;
            }

            var urlRelativePath = new Uri(uri).AbsolutePath.TrimStart('/');
            filePath = UrlRelativePathToFilePath(urlRelativePath);
            if (!File.Exists(filePath))
            {
                return false;
            }

            return true;
        }

        private static string GetAppHostAddress()
        {
#if IOS || MACCATALYST
				// On iOS/MacCatalyst 18 and higher the 0.0.0.0 address does not work, so we use localhost instead.
				// This preserves behavior on older versions of those systems, while defaulting to new behavior on
				// the new system.

				// Note that pre-release versions of iOS/MacCatalyst have the expected Major/Minor values,
				// but the Build, MajorRevision, MinorRevision, and Revision values are all -1, so we need
				// to pass in int.MinValue for those values.

				if (System.OperatingSystem.IsIOSVersionAtLeast(major: 18, minor: int.MinValue, build: int.MinValue) ||
					System.OperatingSystem.IsMacCatalystVersionAtLeast(major: 18, minor: int.MinValue, build: int.MinValue))
				{
					return "localhost";
				}
				else
				{
					return "0.0.0.0";
				}
#else
            return "0.0.0.0";
#endif
        }
    }
}
