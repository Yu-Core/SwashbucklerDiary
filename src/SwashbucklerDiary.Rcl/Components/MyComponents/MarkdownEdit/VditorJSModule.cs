using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Component;

namespace SwashbucklerDiary.Rcl.Components
{
    public class VditorJSModule : JSModuleExtension
    {
        public VditorJSModule(IJSRuntime js) : base(js, "js/vditor-helper.js")
        {
        }

        public async Task After()
        {
            await base.InvokeVoidAsync("after", null);
        }

        public async Task Focus(ElementReference Ref)
        {
            await base.InvokeVoidAsync("focus", Ref);
        }
    }
}
