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
            var base64 = await ScreenshotToBase64(selector);
            return await _appFileManager.CreateTempFileAsync(screenshotFileName, Convert.FromBase64String(base64));
        }
        // TODO: base64还是太慢，应该考虑字节数组
        protected async Task<string> ScreenshotToBase64(string selector)
        {
            var str = await _module.GetScreenshotBase64(selector);
            return str.Substring(str.IndexOf(',') + 1);
        }
    }
}
