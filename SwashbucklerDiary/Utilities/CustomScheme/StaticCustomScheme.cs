namespace SwashbucklerDiary.Utilities
{
    //Windows、Android中将自定义链接替换为./appdata,保证同源，方便截图时不会出现跨域问题
    //macOS、iOS如果未来可以拦截Maui的app://,可以考虑取消appdata://，全都替换为./appdata
    public static class StaticCustomScheme
    {
        public readonly static string CustomStr = "appdata";

        public readonly static string CustomPathPrefix = $"{CustomStr}:///";
#if WINDOWS || ANDROID
        public readonly static string InterceptPrefix = $"/{CustomStr}/";

        private readonly static string LocalPathPrefix = $"./{CustomStr}/";
#endif
        
        public static string LocalPathToCustomPath(string? uri)
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

        public static string CustomPathToLocalPath(string? uri)
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
