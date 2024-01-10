using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.WebAssembly.Extensions
{
    public partial class ServiceCollectionExtensions
    {
        public static async Task<IServiceCollection> AddFileSystem(this IServiceCollection services)
        {
            var JS = services.BuildServiceProvider().GetRequiredService<IJSRuntime>();
            await JS.InvokeVoidAsync("MEMFileSystem.init");
            return services;
        }
    }
}
