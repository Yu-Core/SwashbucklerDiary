using Masa.Blazor.JSModules;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class BackTopButtonJSObjectReference : JSObjectReferenceBase
    {
        public BackTopButtonJSObjectReference(IJSObjectReference jsObjectReference) : base(jsObjectReference)
        {
        }

        public async ValueTask RemoveScrollListener()
        {
            await JSObjectReference.InvokeVoidAsync("removeScrollListener");
        }
    }
}
