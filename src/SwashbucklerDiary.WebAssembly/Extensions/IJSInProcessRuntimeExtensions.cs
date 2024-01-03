using Microsoft.JSInterop;

namespace SwashbucklerDiary.WebAssembly.Extensions
{
    public static class IJSInProcessRuntimeExtensions
    {
        public static ValueTask<IJSInProcessObjectReference> ImportJsModule(this IJSInProcessRuntime jSInProcessRuntime, string path)
        {
            return jSInProcessRuntime.InvokeAsync<IJSInProcessObjectReference>("import", $"./{path}");
        }
    }
}
