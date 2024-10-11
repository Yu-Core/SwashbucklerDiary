using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class MarkdownPreviewJSModule : JSModuleExtension
    {
        public MarkdownPreviewJSModule(IJSRuntime js) : base(js, "js/markdown-preview-helper.js")
        {
        }

        public async Task After(DotNetObjectReference<object> dotNetObjectReference, ElementReference element)
        {
            await InvokeVoidAsync("after", [dotNetObjectReference, element]);
        }

        public async Task CustomRender(ElementReference element)
        {
            await InvokeVoidAsync("customRender", [element]);
        }
    }
}
