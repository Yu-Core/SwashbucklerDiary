using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Services
{
    public class ScreenshotJSModule : CustomJSModule
    {
        public ScreenshotJSModule(IJSRuntime js) : base(js, "js/screenshot.js")
        {
        }

        public async Task<Stream?> GetScreenshotStream(string selector, string proxyUrl)
        {
            var dataReference = await InvokeAsync<IJSStreamReference>("getScreenshotStream", selector, proxyUrl);
            if (dataReference is null) return null;
            return await dataReference.OpenReadStreamAsync(maxAllowedSize: 10_000_000);
        }
    }
}
