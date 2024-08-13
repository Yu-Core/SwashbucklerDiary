using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class WaterfallJSModule : JSModuleExtension
    {
        public WaterfallJSModule(IJSRuntime js) : base(js, "js/waterfall-helper.js")
        {
        }

        public async Task RecordScrollInfo(string selector)
        {
            await InvokeVoidAsync("recordScrollInfo", selector);
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
