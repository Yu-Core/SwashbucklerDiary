using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class ImageWaterfall : MediaWaterfallBase
    {
        private bool showPreviewImage;

        private string? previewImageSrc;

        private DotNetObjectReference<object>? _dotNetObjectReference;

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

            if (!IsDisposed
                && previewMediaElementJSModule is not null
                && firstRender)
            {
                _dotNetObjectReference = DotNetObjectReference.Create<object>(this);
                //图片预览
                await previewMediaElementJSModule.PreviewImage(_dotNetObjectReference, elementReference);
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            _dotNetObjectReference?.Dispose();
        }
    }
}
