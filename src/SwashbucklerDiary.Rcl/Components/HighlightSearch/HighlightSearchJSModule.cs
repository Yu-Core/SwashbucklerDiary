using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class HighlightSearchJSModule : CustomJSModule
    {
        public HighlightSearchJSModule(IJSRuntime js) : base(js, "Components/HighlightSearch/HighlightSearch.razor.js")
        {
        }

        public async ValueTask<HighlightSearchJSObjectReferenceProxy> Init(string? selector,
            DotNetObjectReference<object> dotNetObjectReference)
        {
            var obj = await InvokeAsync<IJSObjectReference>("init", selector, dotNetObjectReference);
            return new HighlightSearchJSObjectReferenceProxy(obj!);
        }
    }
}
