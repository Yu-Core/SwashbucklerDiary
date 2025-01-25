using Microsoft.Extensions.DependencyInjection;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Gtk.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMasaBlazorConfig(this IServiceCollection services)
        {
            var masaBlazorBuilder = services.AddMasaBlazor(Rcl.Extensions.ServiceCollectionExtensions.ConfigMasaBlazorOptions, ServiceLifetime.Singleton);
            masaBlazorBuilder.AddI18nForServer($"wwwroot/_content/{StaticWebAssets.RclAssemblyName}/i18n");
            masaBlazorBuilder.AddI18nForServer($"wwwroot/i18n");

            return services;
        }
    }
}
