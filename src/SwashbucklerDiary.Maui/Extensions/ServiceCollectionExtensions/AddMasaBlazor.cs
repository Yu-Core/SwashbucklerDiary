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
            var language = Microsoft.Maui.Storage.Preferences.Get(Setting.Language.ToString(), "zh-CN");

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
                    },
                    {
                        nameof(MImage), new Dictionary<string, object?>()
                        {
                            { nameof(MImage.Eager), true }
                        }
                    }
                };
                //The reason for setting i18n here is because the Android back button requires i18n to be used
                options.Locale = new BlazorComponent.Locale(language, "en-US");
            }).AddI18nForMauiBlazor($"wwwroot/_content/{StaticWebAssets.RclAssemblyName}/i18n");
            
            return services;
        }
    }
}
