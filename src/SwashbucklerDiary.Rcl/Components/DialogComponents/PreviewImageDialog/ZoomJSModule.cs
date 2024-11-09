using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    internal class ZoomJSModule : JSModuleExtension
    {
        public ZoomJSModule(IJSRuntime js) : base(js, "js/zoom-helper.js")
        {
        }

        public async Task Init(string selector)
        {
            await base.InvokeVoidAsync("initZoom", selector);
        }

        public async Task Reset(string selector)
        {
            await base.InvokeVoidAsync("reset", selector);
        }
    }
}
