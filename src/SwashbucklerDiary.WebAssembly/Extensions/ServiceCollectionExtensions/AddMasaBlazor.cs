using BlazorComponent;
using Masa.Blazor;
using Masa.Blazor.Presets;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.WebAssembly.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static async Task<IServiceCollection> AddMasaBlazorConfig(this IServiceCollection services, string baseAddress)
        {
            var masaBlazorBuilder = services.AddMasaBlazor(options =>
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
            });
            await masaBlazorBuilder.AddI18nForWasmAsync($"{baseAddress}/_content/{StaticWebAssets.RclAssemblyName}/i18n");
            await masaBlazorBuilder.AddI18nForWasmAsync($"{baseAddress}/i18n");

            return services;
        }
    }
}
