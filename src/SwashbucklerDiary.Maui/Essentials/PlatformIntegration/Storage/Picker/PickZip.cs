namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<string?> PickZipFileAsync()
        {
#if WINDOWS
            var types = new[] { ".zip" };
#elif ANDROID
            var types = new[] { "application/zip" };
#elif MACCATALYST || IOS
            var types = new[] { "public.zip-archive" };
#elif TIZEN
            var types = new[] { "*/*" };
#endif
            return PickFileAsync(types, ".zip");
        }
    }
}
