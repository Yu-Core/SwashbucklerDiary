using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class VideoWaterfall : MediaWaterfallBase
    {
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await PreviewMediaElementJSModule.PreviewVideo(elementReference);
            }
        }

        protected override List<ResourceModel> MockRequest()
        {
            var items = base.MockRequest();
            foreach (var item in items)
            {
                item.ResourceUri = HandleSrc(item.ResourceUri);
            }
            return items;
        }

        private static string? HandleSrc(string? src)
        {
            // Display the first frame
            if (!string.IsNullOrEmpty(src) && !src.Contains('#'))
            {
                src += "#t=0.1";
            }

            return src;
        }
    }
}
