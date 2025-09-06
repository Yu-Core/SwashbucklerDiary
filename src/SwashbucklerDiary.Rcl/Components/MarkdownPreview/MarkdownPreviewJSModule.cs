using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class MarkdownPreviewJSModule : CustomJSModule
    {
        public MarkdownPreviewJSModule(IJSRuntime js) : base(js, "js/markdown-preview-helper.js")
        {
        }

        public async Task AfterMarkdown(DotNetObjectReference<object> dotNetObjectReference,
            ElementReference element,
            bool autoPlay,
            ElementReference? outlineElement,
            ElementReference moblieOutlineElement,
            string? linkBase)
        {
            await InvokeVoidAsync("afterMarkdown", [dotNetObjectReference, element, autoPlay, outlineElement, moblieOutlineElement, linkBase]);
        }

        public async Task RenderOutline(ElementReference element, ElementReference? outlineElement)
        {
            await InvokeVoidAsync("renderOutline", [element, outlineElement]);
        }
    }
}
