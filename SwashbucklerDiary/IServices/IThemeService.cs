using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.IServices
{
    public interface IThemeService
    {
        bool System { get; }
        bool Light { get; }
        bool Dark { get; }
        public ThemeState ThemeState { get; set; }
        event Action<ThemeState> OnChanged;
    }
}
