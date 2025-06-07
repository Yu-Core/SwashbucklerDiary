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
            try
            {
            await JSObjectReference.InvokeVoidAsync("removeScrollListener");
        }
            catch (Exception)
            {
            }
        }
    }
}
