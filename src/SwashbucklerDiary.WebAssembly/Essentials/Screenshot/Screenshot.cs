using BlazorComponent.I18n;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Services;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class Screenshot : Rcl.Essentials.IScreenshot
    {
        private readonly ScreenshotJSModule _module;

        private readonly IAppFileManager _appFileManager;

        private readonly string screenshotFileName = "Screenshot.png";

        private readonly IAlertService _alertService;

        private readonly II18nService _i18n;

        public Screenshot(ScreenshotJSModule module,
            IAppFileManager appFileManager,
            IAlertService alertService,
            II18nService i18NService)
        {
            _module = module;
            _appFileManager = appFileManager;
            _alertService = alertService;
            _i18n = i18NService;
        }

        public async Task<string> CaptureAsync(string selector)
        {
            //var base64 = await ScreenshotToBase64(selector);
            //return await _appFileManager.CreateTempFileAsync(screenshotFileName, Convert.FromBase64String(base64));
            await _alertService.Error(_i18n.T("Share.NotSupported"));
            return string.Empty;
        }
        // TODO: base64还是太慢，应该考虑字节数组
        protected async Task<string> ScreenshotToBase64(string selector)
        {
            var str = await _module.GetScreenshotBase64(selector);
            return str.Substring(str.IndexOf(',') + 1);
        }
    }
}
