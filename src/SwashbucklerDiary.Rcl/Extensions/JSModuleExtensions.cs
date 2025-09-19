using Masa.Blazor.JSInterop;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class JSModuleExtensions
    {
        public static async ValueTask DisposeAsync(this JSModule jSModule)
        {
            await (jSModule as IAsyncDisposable).DisposeAsync();
        }

        public static async ValueTask TryDisposeAsync(this JSModule? jSModule)
        {
            if (jSModule is null) return;
            await (jSModule as IAsyncDisposable).DisposeAsync();
        }
    }
}
