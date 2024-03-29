
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class VideoWaterfall : WaterfallBase
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

        protected override List<string?> MockRequest()
        {
            return Value.Skip(srcs.Count).Take(loadCount).Select(it => HandleSrc(it.ResourceUri)).ToList();
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
