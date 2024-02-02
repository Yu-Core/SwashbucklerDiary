using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Essentials
{
    public interface IThemeService
    {
        Theme RealTheme { get; }

        event Action<Theme>? OnChanged;

        Task SetThemeAsync(Theme theme);
    }
}
