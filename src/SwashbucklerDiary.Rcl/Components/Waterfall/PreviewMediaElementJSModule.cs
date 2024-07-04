using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class PreviewMediaElementJSModule : JSModuleExtension
    {
        public PreviewMediaElementJSModule(IJSRuntime js) : base(js, "js/previewMediaElement.js")
        {
        }

        public async Task PreviewImage(DotNetObjectReference<object> dotNetObjectReference, ElementReference element)
        {
            await InvokeVoidAsync("previewImage", dotNetObjectReference, element);
        }

        public async Task PreviewVideo(ElementReference element)
        {
            await InvokeVoidAsync("previewVideo", element);
        }
    }
}
