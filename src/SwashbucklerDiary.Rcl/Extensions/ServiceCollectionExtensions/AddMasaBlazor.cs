using BlazorComponent;
using Masa.Blazor;
using Masa.Blazor.Presets;

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
                    nameof(MDialog), new Dictionary<string, object?>()
                    {
                        { nameof(MDialog.Eager), true }
                    }
                },
                {
                    nameof(MBottomSheet), new Dictionary<string, object?>()
                    {
                        { nameof(MBottomSheet.Eager), true }
                    }
                },
                {
                    nameof(MMenu), new Dictionary<string, object?>()
                    {
                        { nameof(MMenu.Eager), true }
                    }
                }
            };
        }
    }
}
