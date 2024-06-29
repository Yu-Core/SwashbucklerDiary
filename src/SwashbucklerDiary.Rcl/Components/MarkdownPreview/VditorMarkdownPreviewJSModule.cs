using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Component;

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
            Dictionary<string, object>? options)
        {
            await InvokeVoidAsync("previewVditor", [dotNetObjectReference, element, value, options]);
        }

        public async Task RenderLazyLoadingImage(ElementReference element)
        {
            await InvokeVoidAsync("renderLazyLoadingImage", element);
        }
    }
}
