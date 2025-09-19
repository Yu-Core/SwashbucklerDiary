using Masa.Blazor.JSModules;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class HighlightSearchJSObjectReferenceProxy : JSObjectReferenceBase
    {
        public HighlightSearchJSObjectReferenceProxy(IJSObjectReference jsObjectReference) : base(jsObjectReference)
        {
        }

        public async Task Search(string text)
        {
            await JSObjectReference.InvokeVoidAsync("search", text);
        }

        public async Task Down()
        {
            await JSObjectReference.InvokeVoidAsync("down");
        }

        public async Task Up()
        {
            await JSObjectReference.InvokeVoidAsync("up");
        }

        public async ValueTask Clear()
        {
            await JSObjectReference.InvokeVoidAsync("clear");
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await Clear();
            await base.DisposeAsyncCore();
        }
    }
}
