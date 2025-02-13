using Masa.Blazor;
using Microsoft.Extensions.DependencyInjection;
using SwashbucklerDiary.Gtk.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Gtk.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMasaBlazorConfig(this IServiceCollection services)
        {
            static void ConfigMasaBlazorOptions(MasaBlazorOptions options)
            {
                Rcl.Extensions.ServiceCollectionExtensions.ConfigMasaBlazorOptions(options);

                var language = Preferences.Default.Get(nameof(Setting.Language), "zh-CN");
                options.Locale = new(language, "en-US");
            }

            var masaBlazorBuilder = services.AddMasaBlazor(ConfigMasaBlazorOptions, ServiceLifetime.Singleton);
            masaBlazorBuilder.AddI18nForServer($"wwwroot/_content/{Rcl.Essentials.StaticWebAssets.RclAssemblyName}/i18n");
            masaBlazorBuilder.AddI18nForServer($"wwwroot/i18n");

            return services;
        }
    }
}
