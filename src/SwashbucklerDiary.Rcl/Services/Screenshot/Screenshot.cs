using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Services
{
    public class Screenshot : IScreenshot
    {
        private readonly ScreenshotJSModule _module;

        private readonly IAppFileSystem _appFileSystem;

        private readonly IProxyService _proxyService;

        private readonly string screenshotFileName = "Screenshot.png";

        public Screenshot(ScreenshotJSModule module,
            IAppFileSystem appFileSystem,
            IProxyService proxyService)
        {
            _module = module;
            _appFileSystem = appFileSystem;
            _proxyService = proxyService;
        }

        public async Task<string?> CaptureAsync(string selector)
        {
            using var stream = await _module.GetScreenshotStream(selector, _proxyService.ProxyUrl);
            if (stream is null) return null;
            return await _appFileSystem.CreateTempFileAsync(screenshotFileName, stream);
        }
    }
}
