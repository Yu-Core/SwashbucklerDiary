using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    internal class ZoomJSModule : CustomJSModule
    {
        public ZoomJSModule(IJSRuntime js) : base(js, "js/panzoom-helper.js")
        {
        }

        public async Task Init(string selector)
        {
            await base.InvokeVoidAsync("init", selector);
        }

        public async Task Reset(string selector)
        {
            await base.InvokeVoidAsync("reset", selector);
        }
    }
}
