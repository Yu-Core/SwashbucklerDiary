using Masa.Blazor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SwashbucklerDiary.Gtk.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Gtk.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMasaBlazorConfig(this IServiceCollection services)
        {
            services.TryAddSingleton(delegate
            {
                MasaBlazorOptions options = new MasaBlazorOptions();
                Rcl.Extensions.ServiceCollectionExtensions.ConfigMasaBlazorOptions(options);
                var language = Preferences.Default.Get(nameof(Setting.Language), "zh-CN");
                options.Locale = new(language, "en-US");
                return options;
            });

            services.TryAddSingleton<I18n>(sp =>
            {
                return new I18nService(sp.GetRequiredService<MasaBlazorOptions>());
            });

            var masaBlazorBuilder = services.AddMasaBlazor(ServiceLifetime.Singleton);
            masaBlazorBuilder.AddI18nForServer($"wwwroot/_content/{Rcl.Essentials.StaticWebAssets.RclAssemblyName}/i18n");
            masaBlazorBuilder.AddI18nForServer($"wwwroot/i18n");

            return services;
        }
    }
}
