using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class VditorMarkdownPreviewJSModule : CustomJSModule
    {
        public VditorMarkdownPreviewJSModule(IJSRuntime js) : base(js, "js/vditor-preview-helper.js")
        {
        }

        public async Task Preview(DotNetObjectReference<object> dotNetObjectReference,
            ElementReference element,
            string? value,
            Dictionary<string, object>? options,
            ElementReference? outlineElement)
        {
            await InvokeVoidAsync("preview", [dotNetObjectReference, element, value, options, outlineElement]);
        }

        public async Task Md2HTMLPreview(DotNetObjectReference<object> dotNetObjectReference,
            ElementReference element,
            string? value,
            Dictionary<string, object>? options)
        {
            await InvokeVoidAsync("md2htmlPreview", [dotNetObjectReference, element, value, options]);
        }

        public async Task RenderLazyLoadingImage(ElementReference element)
        {
            await InvokeVoidAsync("renderLazyLoadingImage", element);
        }

        public async Task RenderOutline(ElementReference previewElement, ElementReference outlineElement)
        {
            await InvokeVoidAsync("renderOutline", [previewElement, outlineElement]);
        }

        public async Task FixAnchorLinkNavigate(DotNetObjectReference<object> dotNetObjectReference,
            ElementReference previewElement)
        {
            await InvokeVoidAsync("fixAnchorLinkNavigate", [dotNetObjectReference, previewElement]);
        }
    }
}
