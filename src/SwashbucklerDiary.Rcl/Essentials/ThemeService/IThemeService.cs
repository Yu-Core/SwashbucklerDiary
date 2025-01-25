using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Essentials
{
    public interface IThemeService
    {
        Theme RealTheme { get; }

        event Action<Theme>? OnChanged;

        void SetTheme(Theme theme);
    }

    public static class IThemeServiceExtensions
    {
        public static void SetTheme(this IThemeService themeService, int themeInt)
        {
            Theme theme = themeInt switch
            {
                0 => Theme.System,
                1 => Theme.Light,
                2 => Theme.Dark,
                _ => Theme.Light
            };
            themeService.SetTheme(theme);
        }
    }
}
