using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.IServices
{
    public interface IThemeService
    {
        event Action<ThemeState> OnChanged;

        bool Light { get; }

        bool Dark { get; }

        /// <summary>
        /// 主题
        /// </summary>
        ThemeState? ThemeState { get;protected set; }

        /// <summary>
        /// 不包含系统的主题
        /// </summary>
        ThemeState DisplayedThemeState { get; }

        void SetThemeState(ThemeState themeState);

    }
}
