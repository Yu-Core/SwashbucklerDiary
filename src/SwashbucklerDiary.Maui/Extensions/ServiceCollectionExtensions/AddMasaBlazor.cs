using BlazorComponent.I18n;
using Masa.Blazor;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMasaBlazorConfig(this IServiceCollection services)
        {
            services.AddSingleton<I18n>();

            static void optionsAction(MasaBlazorOptions options)
            {
                Rcl.Extensions.ServiceCollectionExtensions.ConfigMasaBlazorOptions(options);
                //The reason for setting i18n here is because the Android back button requires i18n to be used
                var language = Microsoft.Maui.Storage.Preferences.Get(Setting.Language.ToString(), "zh-CN");
                options.Locale = new BlazorComponent.Locale(language, "en-US");
            }

            services.AddMasaBlazor(optionsAction).AddI18nForMauiBlazor($"wwwroot/_content/{StaticWebAssets.RclAssemblyName}/i18n");

            return services;
        }
    }
}
