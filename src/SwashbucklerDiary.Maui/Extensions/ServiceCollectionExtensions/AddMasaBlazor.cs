using BlazorComponent;
using BlazorComponent.I18n;
using Masa.Blazor;
using Masa.Blazor.Presets;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMasaBlazorConfig(this IServiceCollection services)
        {
            services.AddSingleton<I18n>();
            services.AddMasaBlazor(options =>
            {
                options.Defaults = new Dictionary<string, IDictionary<string, object?>?>()
                {
                    {
                        PopupComponents.SNACKBAR, new Dictionary<string, object?>()
                        {
                            { nameof(PEnqueuedSnackbars.Closeable), true },
                            { nameof(PEnqueuedSnackbars.Text), true },
                            { nameof(PEnqueuedSnackbars.Elevation), new StringNumber(2) },
                            { nameof(PEnqueuedSnackbars.Position), SnackPosition.BottomCenter }
                        }
                    }
                };
            }).AddI18nForMauiBlazor($"wwwroot/_content/{StaticWebAssets.RclAssemblyName}/i18n");
            
            return services;
        }
    }
}
