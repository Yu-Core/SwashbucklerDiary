using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class Screenshot : Rcl.Essentials.IScreenshot
    {
        private readonly ScreenshotJSModule _module;

        private readonly IAppFileSystem _appFileSystem;

        private readonly string screenshotFileName = "Screenshot.png";

        private readonly IPopupServiceHelper _popupServiceHelper;

        private readonly II18nService _i18n;

        public Screenshot(ScreenshotJSModule module,
            IAppFileSystem appFileSystem,
            IPopupServiceHelper popupServiceHelper,
            II18nService i18NService)
        {
            _module = module;
            _appFileSystem = appFileSystem;
            _popupServiceHelper = popupServiceHelper;
            _i18n = i18NService;
        }

        public async Task<string> CaptureAsync(string selector)
        {
            //var base64 = await ScreenshotToBase64(selector);
            //return await _appFileSystem.CreateTempFileAsync(screenshotFileName, Convert.FromBase64String(base64));
            await _popupServiceHelper.Error(_i18n.T("Share.NotSupported"));
            return string.Empty;
        }

        protected async Task<string> ScreenshotToBase64(string selector)
        {
            var str = await _module.GetScreenshotBase64(selector);
            return str.Substring(str.IndexOf(',') + 1);
        }
    }
}
