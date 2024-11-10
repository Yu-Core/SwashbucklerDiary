using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class VditorMarkdownPreviewJSModule : JSModuleExtension
    {
        public VditorMarkdownPreviewJSModule(IJSRuntime js) : base(js, "js/vditor-preview-helper.js")
        {
        }

        public async Task PreviewVditor(DotNetObjectReference<object> dotNetObjectReference,
            ElementReference element,
            string? value,
            Dictionary<string, object>? options,
            bool outline,
            ElementReference outlineElement)
        {
            await InvokeVoidAsync("previewVditor", [dotNetObjectReference, element, value, options, outline, outlineElement]);
        }

        public async Task RenderLazyLoadingImage(ElementReference element)
        {
            await InvokeVoidAsync("renderLazyLoadingImage", element);
        }
    }
}
