using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class ImageWaterfall : MediaWaterfallBase
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
                var dotNetObjectReference = DotNetObjectReference.Create<object>(this);
                //图片预览
                await PreviewMediaElementJSModule.PreviewImage(dotNetObjectReference, elementReference);
            }
        }
    }
}
