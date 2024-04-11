
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Extensions;
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
                module = await JS.ImportRclJsModule("js/previewMediaElement.js");
                await module.InvokeVoidAsync("previewVideo", elementReference);
            }
        }

        protected override List<ResourceModel> MockRequest(int requestCount = 0)
        {
            var items = base.MockRequest(requestCount);
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
