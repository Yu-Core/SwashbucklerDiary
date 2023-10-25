namespace SwashbucklerDiary.Utilities
{
    //Windows、Android中自定义链接为./appdata/,保证同源，方便截图时不会出现跨域问题
    //macOS、iOS暂时还不可以，除非未来可以拦截Maui的app://
    public static class StaticCustomPath
    {
        public readonly static string CustomStr = "appdata";

        public readonly static string CustomPathPrefix = $"./{CustomStr}/";

        public readonly static string InterceptPrefix = $"/{CustomStr}/";
    }
}
