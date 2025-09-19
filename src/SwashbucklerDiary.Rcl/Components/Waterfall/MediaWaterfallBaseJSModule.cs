using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class MediaWaterfallBaseJSModule : CustomJSModule
    {
        public MediaWaterfallBaseJSModule(IJSRuntime js) : base(js, "Components/Waterfall/MediaWaterfallBase.Razor.js")
        {
        }

        public async Task StartRecordScrollInfo(string selector)
        {
            await InvokeVoidAsync("startRecordScrollInfo", selector);
        }

        public async Task StopRecordScrollInfo(string selector)
        {
            await InvokeVoidAsync("stopRecordScrollInfo", selector);
        }

        public async Task RestoreScrollPosition(string selector)
        {
            await InvokeVoidAsync("restoreScrollPosition", selector);
        }
    }
}
