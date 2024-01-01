using Microsoft.JSInterop;

namespace SwashbucklerDiary.WebAssembly.Extensions
{
    public partial class ServiceCollectionExtensions
    {
        public static async Task<IServiceCollection> AddFileSystem(this IServiceCollection services)
        {
            var JS = services.BuildServiceProvider().GetRequiredService<IJSRuntime>();
            var module = await JS.InvokeAsync<IJSObjectReference>("import", "./js/fileSystem.js");
            await module.InvokeVoidAsync("synchronizeFileWithIDBFS");
            return services;
        }
    }
}
