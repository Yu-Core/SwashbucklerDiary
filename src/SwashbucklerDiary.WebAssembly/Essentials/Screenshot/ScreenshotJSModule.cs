using Masa.Blazor.JSInterop;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class ScreenshotJSModule : JSModule
    {
        public ScreenshotJSModule(IJSRuntime js)
            : base(js, "./js/screenshot.js")
        {
        }

        public async Task<string> GetScreenshotBase64(string selector)
        {
            return await InvokeAsync<string>("getScreenshotBase64", selector);
        }
    }
}
