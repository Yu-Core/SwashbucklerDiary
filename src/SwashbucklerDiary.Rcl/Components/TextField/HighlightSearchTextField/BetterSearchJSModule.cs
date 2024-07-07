using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class BetterSearchJSModule : JSModuleExtension
    {
        public BetterSearchJSModule(IJSRuntime js) : base(js, "js/bettersearch-proxy.js")
        {
        }

        public async ValueTask<BetterSearchJSObjectReferenceProxy> Init(string? selector)
        {
            var obj = await InvokeAsync<IJSObjectReference>("init", selector);
            return new BetterSearchJSObjectReferenceProxy(obj!);
        }
    }
}
