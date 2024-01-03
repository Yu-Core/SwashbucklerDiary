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

        public async Task PreventInputLoseFocus()
        {
            //点击工具栏不会丢失焦点
            await base.InvokeVoidAsync("preventInputLoseFocus", null);
        }

        public async Task MoveCursorForward(int length)
        {
            await base.InvokeVoidAsync("moveCursorForward", length);
        }

        public async Task Focus(ElementReference Ref)
        {
            await base.InvokeVoidAsync("focus", Ref);
        }
    }
}
