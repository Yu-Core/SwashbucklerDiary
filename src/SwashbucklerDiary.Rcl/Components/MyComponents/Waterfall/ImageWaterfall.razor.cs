using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class ImageWaterfall : WaterfallBase
    {
        private bool showPreviewImage;

        private string? previewImageSrc;

        [JSInvokable]
        public async Task PreviewImage(string src)
        {
            previewImageSrc = src;
            showPreviewImage = true;
            await InvokeAsync(StateHasChanged);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                module = await JS.ImportRclJsModule("js/previewMediaElement.js");
                var dotNetCallbackRef = DotNetObjectReference.Create(this);

                //图片预览
                await module.InvokeVoidAsync("previewImage", [dotNetCallbackRef, "PreviewImage", elementReference]);
            }
        }
    }
}
