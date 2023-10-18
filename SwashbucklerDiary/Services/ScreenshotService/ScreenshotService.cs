using Microsoft.JSInterop;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Services
{
    public class ScreenshotService : IScreenshotService
    {
        private readonly ScreenshotJSModule module;

        public ScreenshotService(IJSRuntime js, IAppDataService appDataService)
        {
            module = new(js, appDataService);
        }

        public async Task<string> ScreenshotToBase64(string selector)
        {
            var str = await module.GetScreenshotBase64(selector);
            return str.Substring(str.IndexOf(",") + 1);
        }
    }
}
