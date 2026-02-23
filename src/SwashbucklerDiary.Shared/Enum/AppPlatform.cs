namespace SwashbucklerDiary.Shared
{
    public enum AppPlatform
    {
        Unknown,
        Windows,
        Android,
        iOS,
        macOS,
        Tizen,
        BrowserWasm,
        Linux,
        BrowserServer,
    }

    public static class AppPlatformExtensions
    {
        public static bool IsBrowser(this AppPlatform appPlatform)
        {
            return appPlatform switch
            {
                AppPlatform.BrowserWasm => true,
                AppPlatform.BrowserServer => true,
                _ => false
            };
        }
    }
}
