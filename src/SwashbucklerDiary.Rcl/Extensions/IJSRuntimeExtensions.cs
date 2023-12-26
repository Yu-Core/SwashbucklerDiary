using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class IJSRuntimeExtensions
    {
        public static ValueTask<IJSObjectReference> ImportJsModule(this IJSRuntime jSRuntime, string path)
        {
            return jSRuntime.InvokeAsync<IJSObjectReference>("import", $"./_content/{StaticWebAssets.RclAssemblyName}/{path}");
        }
    }
}
