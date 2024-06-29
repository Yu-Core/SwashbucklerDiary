using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Component;

namespace SwashbucklerDiary.Rcl.Components
{
    public class TextareaJSModule : JSModuleExtension
    {
        public TextareaJSModule(IJSRuntime js) : base(js, "js/mtextarea-helper.js")
        {
        }

        public ValueTask<string?> InsertText(ElementReference elementReference, string value)
        {
            return base.InvokeAsync<string?>("insertText", [elementReference, value]);
        }
    }
}
