using Masa.Blazor;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Maui.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMasaBlazorConfig(this IServiceCollection services)
        {
            services.TryAddSingleton(delegate
            {
                MasaBlazorOptions options = new MasaBlazorOptions();
                Rcl.Extensions.ServiceCollectionExtensions.ConfigMasaBlazorOptions(options);
                //The reason for setting i18n here is because the Android back button requires i18n to be used
                var language = Microsoft.Maui.Storage.Preferences.Get(nameof(Setting.Language), "zh-CN");
                options.Locale = new(language, "en-US");
                return options;
            });

            services.TryAddSingleton<I18n>(sp =>
            {
                return new I18nService(sp.GetRequiredService<MasaBlazorOptions>());
            });

            var masaBlazorBuilder = services.AddMasaBlazor(ServiceLifetime.Singleton);
            masaBlazorBuilder.AddI18nForMauiBlazor($"wwwroot/_content/{StaticWebAssets.RclAssemblyName}/i18n");
            masaBlazorBuilder.AddI18nForMauiBlazor($"wwwroot/i18n");

            return services;
        }
    }
}
