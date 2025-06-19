using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class InputJSModule : CustomJSModule
    {
        public InputJSModule(IJSRuntime js) : base(js, "js/input-helper.js")
        {
        }

        public ValueTask<string?> InsertText(ElementReference elementReference, string value)
        {
            return base.InvokeAsync<string?>("insertText", [elementReference, value]);
        }
    }
}
