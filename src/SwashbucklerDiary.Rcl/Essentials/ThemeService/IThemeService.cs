using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Essentials
{
    public interface IThemeService
    {
        Theme RealTheme { get; }

        event Func<Theme, Task>? OnChanged;

        Task SetThemeAsync(Theme theme);
    }
}
