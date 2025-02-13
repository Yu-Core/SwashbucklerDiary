using Masa.Blazor;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.WebAssembly.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static async Task<IServiceCollection> AddMasaBlazorConfig(this IServiceCollection services, string baseAddress)
        {
            services.TryAddSingleton(delegate
            {
                MasaBlazorOptions masaBlazorOptions = new MasaBlazorOptions();
                Rcl.Extensions.ServiceCollectionExtensions.ConfigMasaBlazorOptions(masaBlazorOptions);
                return masaBlazorOptions;
            });

            services.TryAddSingleton<I18n>();

            var masaBlazorBuilder = services.AddMasaBlazor(ServiceLifetime.Singleton);
            await masaBlazorBuilder.AddI18nForWasmAsync($"{baseAddress}/_content/{StaticWebAssets.RclAssemblyName}/i18n");
            await masaBlazorBuilder.AddI18nForWasmAsync($"{baseAddress}/i18n");

            return services;
        }
    }
}
