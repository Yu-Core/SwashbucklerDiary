using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class IJSRuntimeExtensions
    {
        public static ValueTask<IJSObjectReference> ImportRclJsModule(this IJSRuntime jSRuntime, string path)
        {
            return jSRuntime.InvokeAsync<IJSObjectReference>("import", $"./_content/{StaticWebAssets.RclAssemblyName}/{path}");
        }

        public static ValueTask<IJSObjectReference> ImportJsModule(this IJSRuntime jSRuntime, string path)
        {
            return jSRuntime.InvokeAsync<IJSObjectReference>("import", $"./{path}");
        }

        public static ValueTask HistoryReplaceState(this IJSRuntime jSRuntime, string uri)
        {
            return jSRuntime.InvokeVoidAsync("history.replaceState", [null, null, uri]);
        }

        public static ValueTask HistoryBack(this IJSRuntime jSRuntime)
        {
            return jSRuntime.InvokeVoidAsync("history.back");
        }

        public static ValueTask HistoryGo(this IJSRuntime jSRuntime, int delta)
        {
            return jSRuntime.InvokeVoidAsync("history.go", delta);
        }

        public static ValueTask<int> HistoryLength(this IJSRuntime jSRuntime)
        {
            return jSRuntime.InvokeAsync<int>("historyLength");
        }
    }
}
