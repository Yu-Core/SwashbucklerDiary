using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.WebAssembly.Extensions
{
    public partial class ServiceCollectionExtensions
    {
        public static async Task<IServiceCollection> AddFileSystem(this IServiceCollection services)
        {
            var JS = services.BuildServiceProvider().GetRequiredService<IJSRuntime>();
            var module = await JS.ImportJsModule("js/fileSystem.js");
            await module.InvokeVoidAsync("initFileSystem");
            await module.DisposeAsync();
            return services;
        }
    }
}
