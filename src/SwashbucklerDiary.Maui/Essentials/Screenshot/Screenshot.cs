using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui.Essentials
{
    public class Screenshot : Rcl.Essentials.IScreenshot
    {
        private readonly ScreenshotJSModule _module;

        private readonly IAppFileManager _appFileManager;

        private readonly string screenshotFileName = "Screenshot.png";

        public Screenshot(ScreenshotJSModule module,
            IAppFileManager appFileManager)
        {
            _module = module;
            _appFileManager = appFileManager;
        }

        public async Task<string> CaptureAsync(string selector)
        {
            using var stream = await _module.GetScreenshotStream(selector);
            return await _appFileManager.CreateTempFileAsync(screenshotFileName, stream);
        }
    }
}
