using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class MarkdownJSModule : CustomJSModule
    {
        public MarkdownJSModule(IJSRuntime js) : base(js, "Components/MarkdownEdit/MarkdownEdit.razor.js")
        {
        }

        public async Task After(DotNetObjectReference<object> dotNetObjectReference, ElementReference element, ElementReference outlineElement, bool copyCutPatch)
        {
            await base.InvokeVoidAsync("after", [dotNetObjectReference, element, outlineElement, copyCutPatch]);
        }

        public async Task Focus(ElementReference element)
        {
            await base.InvokeVoidAsync("focus", element);
        }

        public async Task FocusToEnd(ElementReference element)
        {
            await base.InvokeVoidAsync("focusToEnd", element);
        }

        public async Task Upload(ElementReference element, ElementReference? inputFileElement)
        {
            await base.InvokeVoidAsync("upload", element, inputFileElement);
        }

        public async Task SetMoblieOutline(ElementReference element, ElementReference outlineElement)
        {
            await base.InvokeVoidAsync("setMoblieOutline", [element, outlineElement]);
        }
    }
}
