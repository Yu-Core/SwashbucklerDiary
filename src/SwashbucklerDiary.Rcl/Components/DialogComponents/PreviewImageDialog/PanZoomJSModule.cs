using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class PanzoomJSModule : CustomJSModule
    {
        public PanzoomJSModule(IJSRuntime js) : base(js, "js/panzoom-helper.js")
        {
        }

        public async Task Init(ElementReference elementReference)
        {
            await base.InvokeVoidAsync("init", elementReference);
        }

        public async Task Reset(ElementReference elementReference)
        {
            await base.InvokeVoidAsync("reset", elementReference);
        }
    }
}
