using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class Screenshot : Rcl.Essentials.IScreenshot
    {
        private readonly ScreenshotJSModule _module;

        private readonly IAppFileSystem _appFileSystem;

        private readonly string screenshotFileName = "Screenshot.png";

        private readonly IAlertService _alertService;

        private readonly II18nService _i18n;

        public Screenshot(ScreenshotJSModule module,
            IAppFileSystem appFileSystem,
            IAlertService alertService,
            II18nService i18NService)
        {
            _module = module;
            _appFileSystem = appFileSystem;
            _alertService = alertService;
            _i18n = i18NService;
        }

        public async Task<string> CaptureAsync(string selector)
        {
            //var base64 = await ScreenshotToBase64(selector);
            //return await _appFileSystem.CreateTempFileAsync(screenshotFileName, Convert.FromBase64String(base64));
            await _alertService.Error(_i18n.T("Not supported on the current platform"));
            return string.Empty;
        }

        protected async Task<string> ScreenshotToBase64(string selector)
        {
            var str = await _module.GetScreenshotBase64(selector);
            return str.Substring(str.IndexOf(',') + 1);
        }
    }
}
