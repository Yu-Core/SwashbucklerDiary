using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Essentials
{
    public interface IThemeService
    {
        event Func<Theme, Task>? OnChanged;

        Task SetThemeAsync(Theme theme);

        ValueTask<Theme> GetThemeAsync();
    }
}
