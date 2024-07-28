using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class BackTopButtonJSModule : JSModuleExtension
    {
        public BackTopButtonJSModule(IJSRuntime js) : base(js, "Components/FloatButton/BackTopButton.razor.js")
        {
        }

        public ValueTask<IJSObjectReference> Init(string selector, ElementReference? element, DotNetObjectReference<object>? dotNetObject)
        {
            return InvokeAsync<IJSObjectReference>("init", selector, element, dotNetObject)!;
        }
    }
}
