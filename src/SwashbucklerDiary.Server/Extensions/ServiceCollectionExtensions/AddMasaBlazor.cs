using Masa.Blazor;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Server.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMasaBlazorConfig(this IServiceCollection services)
        {
            services.TryAddScoped(delegate
            {
                MasaBlazorOptions options = new MasaBlazorOptions();
                Rcl.Extensions.ServiceCollectionExtensions.ConfigMasaBlazorOptions(options);
                //var language = Microsoft.Maui.Storage.Preferences.Default.Get(nameof(Setting.Language), "zh-CN");
                //var culture = new CultureInfo(language);
                //options.Locale = new(culture);
                //options.RTL = culture.TextInfo.IsRightToLeft;

                return options;
            });

            services.TryAddScoped<I18n>(sp =>
            {
                return new I18nService(sp.GetRequiredService<MasaBlazorOptions>());
            });

            var masaBlazorBuilder = services.AddMasaBlazor(ServiceLifetime.Scoped);
            masaBlazorBuilder.AddI18nForServer($"wwwroot/_content/{Rcl.Essentials.StaticWebAssets.RclAssemblyName}/i18n");

            return services;
        }
    }
}
