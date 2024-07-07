using Masa.Blazor.JSModules;
using Microsoft.JSInterop;
using System.Text.Json;

namespace SwashbucklerDiary.Rcl.Components
{
    public class BetterSearchJSObjectReferenceProxy : JSObjectReferenceBase
    {
        public BetterSearchJSObjectReferenceProxy(IJSObjectReference jsObjectReference) : base(jsObjectReference)
        {
        }

        public int Count { get; private set; }

        public int SearchIndex { get; private set; }

        public async Task Search(string text)
        {
            await JSObjectReference.InvokeVoidAsync("search", text);
            await UpdateProperties();
        }

        public async Task Down()
        {
            await JSObjectReference.InvokeVoidAsync("down");
            await UpdateProperties();
        }

        public async Task Up()
        {
            await JSObjectReference.InvokeVoidAsync("up");
            await UpdateProperties();
        }

        public async ValueTask Clear()
        {
            await JSObjectReference.InvokeVoidAsync("clear");
            await UpdateProperties();
        }

        protected override ValueTask DisposeAsyncCore()
        {
            return Clear();
        }

        private async Task UpdateProperties()
        {
            var properties = await JSObjectReference.InvokeAsync<Dictionary<string, JsonElement>>("updateProperties");
            if (properties.TryGetValue("count", out JsonElement count))
            {
                Count = count.Deserialize<int>();
            }

            if (properties.TryGetValue("searchIndex", out JsonElement searchIndex))
            {
                SearchIndex = searchIndex.Deserialize<int>();
            }
        }
    }
}
