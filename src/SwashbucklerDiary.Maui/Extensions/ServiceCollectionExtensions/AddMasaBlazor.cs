using Masa.Blazor;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Maui.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMasaBlazorConfig(this IServiceCollection services)
        {
            static void ConfigMasaBlazorOptions(MasaBlazorOptions options)
            {
                Rcl.Extensions.ServiceCollectionExtensions.ConfigMasaBlazorOptions(options);
                //The reason for setting i18n here is because the Android back button requires i18n to be used
                var language = Microsoft.Maui.Storage.Preferences.Get(nameof(Setting.Language), "zh-CN");
                options.Locale = new(language, "en-US");
            }

            var masaBlazorBuilder = services.AddMasaBlazor(ConfigMasaBlazorOptions, ServiceLifetime.Singleton);
            masaBlazorBuilder.AddI18nForMauiBlazor($"wwwroot/_content/{StaticWebAssets.RclAssemblyName}/i18n");
            masaBlazorBuilder.AddI18nForMauiBlazor($"wwwroot/i18n");

            return services;
        }
    }
}
