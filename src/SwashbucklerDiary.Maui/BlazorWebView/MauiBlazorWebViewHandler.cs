using Microsoft.AspNetCore.Components.WebView.Maui;
using System.Reflection;

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

        private static string GetAppHostAddress()
        {
            Type type = typeof(Microsoft.AspNetCore.Components.WebView.Maui.BlazorWebView);
            PropertyInfo propertyInfo = type.GetProperty("AppHostAddress", BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new Exception("Property AppHostAddress does not exist");

            return (string?)propertyInfo.GetValue(null)!;
        }

        private static bool InterceptLocalFileRequest(string uri, out string filePath)
        {
            if (!uri.StartsWith(BaseUri))
            {
                filePath = string.Empty;
                return false;
            }

            var urlRelativePath = new Uri(uri).AbsolutePath.TrimStart('/');
            filePath = LocalFileWebAccessHelper.UrlRelativePathToFilePath(urlRelativePath);
            if (!File.Exists(filePath))
            {
                return false;
            }

            return true;
        }

        private static int ParseRange(string rangeString, ref long rangeStart, ref long rangeEnd)
        {
            var ranges = rangeString.Split('=');
            if (ranges.Length < 2 || string.IsNullOrEmpty(ranges[1]))
            {
                return 0;
            }

            string[] rangeDatas = ranges[1].Split("-");
            rangeStart = Convert.ToInt64(rangeDatas[0]);
            if (rangeDatas.Length > 1 && !string.IsNullOrEmpty(rangeDatas[1]))
            {
                rangeEnd = Convert.ToInt64(rangeDatas[1]);
                return 2;
            }
            else
            {
                return 1;
            }
        }
    }
}
