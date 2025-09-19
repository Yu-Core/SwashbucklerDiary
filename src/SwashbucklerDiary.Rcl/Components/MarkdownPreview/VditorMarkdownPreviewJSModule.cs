using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class VditorMarkdownPreviewJSModule : CustomJSModule
    {
        public VditorMarkdownPreviewJSModule(IJSRuntime js) : base(js, "Components/MarkdownPreview/VditorMarkdownPreview.razor.js")
        {
        }

        public async Task Preview(DotNetObjectReference<object> dotNetObjectReference,
            ElementReference element,
            string? value,
            Dictionary<string, object>? options,
            bool optimize)
        {
            await InvokeVoidAsync("preview", [dotNetObjectReference, element, value, options, optimize]);
        }

        public async Task Md2HTMLPreview(DotNetObjectReference<object> dotNetObjectReference,
            ElementReference element,
            string? value,
            Dictionary<string, object>? options,
            bool optimize)
        {
            await InvokeVoidAsync("md2htmlPreview", [dotNetObjectReference, element, value, options, optimize]);
        }

        public async Task RenderLazyLoadingImage(ElementReference element)
        {
            await InvokeVoidAsync("renderLazyLoadingImage", element);
        }

        public async Task RenderOutline(ElementReference previewElement, ElementReference outlineElement)
        {
            await InvokeVoidAsync("renderOutline", [previewElement, outlineElement]);
        }
    }
}
