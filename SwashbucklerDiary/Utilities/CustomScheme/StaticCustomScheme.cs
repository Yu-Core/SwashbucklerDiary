namespace SwashbucklerDiary.Utilities
{
    //Windows、Android中将自定义链接替换为https://0.0.0.0/appdata,保证同源，方便截图时不会出现跨域问题
    //macOS、iOS用不了https://，只能用appdata://，所以截图时出现跨域问题
    public static class StaticCustomScheme
    {
        public readonly static string CustomStr = "appdata";

        public readonly static string CustomPathPrefix = $"{CustomStr}:///";
#if WINDOWS || ANDROID
        public readonly static string InterceptPrefix = $"/{CustomStr}/";

        private readonly static string LocalPathPrefix = $"./{CustomStr}/";
#endif
        

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
