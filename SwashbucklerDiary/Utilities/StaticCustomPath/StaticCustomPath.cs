namespace SwashbucklerDiary.Utilities
{
    //自定义链接为appdata/
    //等同于https://0.0.0.0/appdata/或app://0.0.0.0/appdata/
    //保证与页面同源，方便截图时不会出现跨域问题
    public static class StaticCustomPath
    {
        private const string AppHostAddress = "0.0.0.0";
#if IOS || MACCATALYST
        private const string AppOrigin = $"app://{AppHostAddress}/";
#else
        private const string AppOrigin = $"https://{AppHostAddress}/";
#endif

        public readonly static string CustomStr = "appdata";

        public readonly static string CustomPathPrefix = $"{CustomStr}/";

        public readonly static string InterceptPrefix = $"{AppOrigin}{CustomPathPrefix}";
    }
}
