using BlazorComponent;
using Masa.Blazor;
using Masa.Blazor.Presets;
using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static void ConfigMasaBlazorOptions(MasaBlazorOptions options)
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
                },
                {
                    nameof(MDialogExtension), new Dictionary<string, object?>()
                    {
                        { nameof(MDialogExtension.Eager), true }
                    }
                },
                {
                    nameof(MBottomSheetExtension), new Dictionary<string, object?>()
                    {
                        { nameof(MBottomSheetExtension.Eager), true }
                    }
                },
                {
                    nameof(MMenuExtension), new Dictionary<string, object?>()
                    {
                        { nameof(MMenuExtension.Eager), true }
                    }
                }
            };
        }
    }
}
