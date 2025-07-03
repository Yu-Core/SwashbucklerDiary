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
                        { nameof(PEnqueuedSnackbars.Text), true },
                        { nameof(PEnqueuedSnackbars.Elevation), new StringNumber(2) },
                        { nameof(PEnqueuedSnackbars.Position), SnackPosition.TopCenter }
                    }
                },
                {
                    nameof(MBottomSheet), new Dictionary<string, object?>()
                    {
                        { nameof(MBottomSheet.Eager), true }
                    }
                },
                {
                    nameof(MList), new Dictionary<string, object?>()
                    {
                        { nameof(MList.Slim), true }
                    }
                }
            };
            options.ConfigureTheme(theme =>
            {
                theme.Themes.Light.Surface = ThemeColor.LightSurface;
                theme.Themes.Light.Variables.BorderOpacity = 0.06f;
                theme.Themes.Dark.Surface = ThemeColor.DarkSurface;
            });
            options.ConfigureIcons("MaterialSymbols", new MaterialSymbolsAliases());
        }
    }
}
