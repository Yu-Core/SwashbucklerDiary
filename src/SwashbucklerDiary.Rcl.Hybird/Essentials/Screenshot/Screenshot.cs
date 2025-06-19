using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Hybird.Essentials
{
    public class Screenshot : Rcl.Essentials.IScreenshot
    {
        private readonly ScreenshotJSModule _module;

        private readonly IAppFileSystem _appFileSystem;

        private readonly string screenshotFileName = "Screenshot.png";

        public Screenshot(ScreenshotJSModule module,
            IAppFileSystem appFileSystem)
        {
            _module = module;
            _appFileSystem = appFileSystem;
        }

        public async Task<string> CaptureAsync(string selector)
        {
            using var stream = await _module.GetScreenshotStream(selector);
            return await _appFileSystem.CreateTempFileAsync(screenshotFileName, stream);
        }
    }
}
