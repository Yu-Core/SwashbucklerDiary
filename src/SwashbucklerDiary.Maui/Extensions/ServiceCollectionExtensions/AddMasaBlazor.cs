using BlazorComponent.I18n;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMasaBlazorConfig(this IServiceCollection services)
        {
            services.AddSingleton<I18n>();
            var language = Microsoft.Maui.Storage.Preferences.Get(Setting.Language.ToString(), "zh-CN");

            services.AddMasaBlazor(options =>
            {
                Rcl.Extensions.ServiceCollectionExtensions.ConfigMasaBlazorOptions(options);
                //The reason for setting i18n here is because the Android back button requires i18n to be used
                options.Locale = new BlazorComponent.Locale(language, "en-US");

            }).AddI18nForMauiBlazor($"wwwroot/_content/{StaticWebAssets.RclAssemblyName}/i18n");

            return services;
        }
    }
}
