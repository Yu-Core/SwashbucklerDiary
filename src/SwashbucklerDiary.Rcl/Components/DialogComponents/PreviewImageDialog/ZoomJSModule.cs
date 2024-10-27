using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    internal class ZoomJSModule : JSModuleExtension
    {
        public ZoomJSModule(IJSRuntime js) : base(js, "js/zoom-helper.js")
        {
        }

        public async Task Init(ElementReference element)
        {
            await base.InvokeVoidAsync("initZoom", element);
        }

        public async Task Reset(ElementReference element)
        {
            await base.InvokeVoidAsync("reset", element);
        }
    }
}
