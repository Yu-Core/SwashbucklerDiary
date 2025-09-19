namespace SwashbucklerDiary.Rcl.Components
{
    public partial class VideoWaterfall : MediaWaterfallBase
    {
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!IsDisposed
                && previewMediaElementJSModule is not null
                && firstRender)
            {
                await previewMediaElementJSModule.PreviewVideo(elementReference);
            }
        }
    }
}
