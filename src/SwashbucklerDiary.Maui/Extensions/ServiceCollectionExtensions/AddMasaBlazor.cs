using Masa.Blazor;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMasaBlazorConfig(this IServiceCollection services)
        {
            static void optionsAction(MasaBlazorOptions options)
            {
                Rcl.Extensions.ServiceCollectionExtensions.ConfigMasaBlazorOptions(options);
                //The reason for setting i18n here is because the Android back button requires i18n to be used
                var language = Microsoft.Maui.Storage.Preferences.Get(Setting.Language.ToString(), "zh-CN");
                options.Locale = new(language, "en-US");
            }

            var masaBlazorBuilder = services.AddMasaBlazor(optionsAction, ServiceLifetime.Singleton);
            masaBlazorBuilder.AddI18nForMauiBlazor($"wwwroot/_content/{StaticWebAssets.RclAssemblyName}/i18n");
            masaBlazorBuilder.AddI18nForMauiBlazor($"wwwroot/i18n");

            return services;
        }
    }
}
