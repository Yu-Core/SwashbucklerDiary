using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.IServices
{
    public interface IThemeService
    {
        bool Light { get; }
        bool Dark { get; }
        public ThemeState ThemeState { get;}
        void SetThemeState(ThemeState themeState);
        event Action<ThemeState> OnChanged;
    }
}
