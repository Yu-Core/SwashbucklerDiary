using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.WebAssembly.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static async Task<IServiceCollection> AddMasaBlazorConfig(this IServiceCollection services, string baseAddress)
        {
            var masaBlazorBuilder = services.AddMasaBlazor(Rcl.Extensions.ServiceCollectionExtensions.ConfigMasaBlazorOptions);
            await masaBlazorBuilder.AddI18nForWasmAsync($"{baseAddress}/_content/{StaticWebAssets.RclAssemblyName}/i18n");
            await masaBlazorBuilder.AddI18nForWasmAsync($"{baseAddress}/i18n");

            return services;
        }
    }
}
