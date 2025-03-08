using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class BackTopButtonJSModule : CustomJSModule
    {
        public BackTopButtonJSModule(IJSRuntime js) : base(js, "Components/FloatButton/BackTopButton.razor.js")
        {
        }

        public async ValueTask<BackTopButtonJSObjectReference> Init(string selector, ElementReference? element, DotNetObjectReference<object>? dotNetObject)
        {
            var jSObjectReference = await InvokeAsync<IJSObjectReference>("init", selector, element, dotNetObject);
            return new BackTopButtonJSObjectReference(jSObjectReference);
        }
    }
}
