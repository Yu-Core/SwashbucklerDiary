using Microsoft.AspNetCore.Components.WebView.Maui;

namespace SwashbucklerDiary.Utilities
{
    //Android中将自定义链接替换为https://0.0.0.0/appdata,保证同源，方便截图时不会出现跨域问题
    //Windows中将自定义链接替换为虚拟路径https://appdata，如果使用拦截的话，会有文件大小限制，超出9MB的图片不会显示，截图时没有跨域问题，不需要同源
    public static class StaticCustomScheme
    {
        public readonly static string CustomStr = "appdata";
#if WINDOWS
        private readonly static string LocalPathPrefix = $"https://{CustomStr}/";
#elif ANDROID
        private readonly static string LocalPathPrefix = $"./{CustomStr}/";
#endif
        public readonly static string CustomPathPrefix = $"{CustomStr}:///";

        public static string ReverseCustomSchemeRender(string? uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return string.Empty;
            }

#if WINDOWS || ANDROID
            uri = uri.Replace(LocalPathPrefix, CustomPathPrefix);
#endif
            return uri;
        }

        public static string CustomSchemeRender(string? uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return string.Empty;
            }

#if WINDOWS || ANDROID
            uri = uri.Replace(CustomPathPrefix, LocalPathPrefix);
#endif
            return uri;
        }

        public static bool IsInternalUri(string uri)
        {
            if (uri.StartsWith(CustomPathPrefix))
            {
                return true;
            }
#if WINDOWS || ANDROID
            if (uri.StartsWith(LocalPathPrefix))
            {
                return true;
            }
#endif
            return false;
        }
    }
}
