using Masa.Blazor;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.WebAssembly.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static async Task<IServiceCollection> AddMasaBlazorConfig(this IServiceCollection services, string baseAddress)
        {
            services.TryAddSingleton(delegate
            {
                MasaBlazorOptions options = new MasaBlazorOptions();
                Rcl.Extensions.ServiceCollectionExtensions.ConfigMasaBlazorOptions(options);
                return options;
            });

            services.TryAddSingleton<I18n>(sp =>
            {
                return new I18nService(sp.GetRequiredService<MasaBlazorOptions>());
            });

            var masaBlazorBuilder = services.AddMasaBlazor(ServiceLifetime.Singleton);
            await masaBlazorBuilder.AddI18nForWasmAsync(Path.Combine(baseAddress, "_content", StaticWebAssets.RclAssemblyName, "i18n"));

            return services;
        }
    }
}
