using SwashbucklerDiary.Maui.Essentials;
using SwashbucklerDiary.Rcl;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui
{
    public partial class App : Application
    {
        private Color backgroundColor = default!;

        private readonly IAppLifecycle _appLifecycle;

        private readonly INavigateController _navigateController;

        private readonly IThemeService _themeService;

        private readonly Masa.Blazor.MasaBlazor _masaBlazor;

        private readonly II18nService _i18n;

        public App(IAppLifecycle appLifecycle,
            INavigateController navigateController,
            IThemeService themeService,
            Masa.Blazor.MasaBlazor masaBlazor,
            II18nService i18n)
        {
            InitializeComponent();

            _themeService = themeService;
            _navigateController = navigateController;
            _masaBlazor = masaBlazor;
            _appLifecycle = appLifecycle;
            _i18n = i18n;
            InitTheme();
            InitAppActions();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new MainPage(backgroundColor, _navigateController, _appLifecycle));
            window.Resumed += (s, e) => _appLifecycle.Resume();
            window.Stopped += (s, e) => _appLifecycle.Stop();
            window.Created += WindowCreated;
            window.Title = _i18n.T("Swashbuckler Diary");
            _i18n.CultureChanged += (_, _) => SetTitle();

            return window;
        }

        protected void WindowCreated(object? sender, EventArgs eventArgs)
        {
            ThemeChanged(_themeService.RealTheme);
        }

        private void InitTheme()
        {
            var themeInt = Microsoft.Maui.Storage.Preferences.Default.Get<int>(nameof(Setting.Theme), 0);
            var theme = (Theme)themeInt;
            _themeService.SetTheme(theme);

            _themeService.OnChanged += ThemeChanged;

            bool dark = _themeService.RealTheme == Shared.Theme.Dark;
            backgroundColor = Color.FromArgb(dark ? ThemeColor.DarkSurface : ThemeColor.LightSurface);
        }

        private void ThemeChanged(Theme theme)
        {
            _masaBlazor.SetTheme(theme == Theme.Dark);
            TitleBarOrStatusBar.SetTitleBarOrStatusBar(theme);
        }

        private void InitAppActions()
        {
            SetAppActions();
            _i18n.CultureChanged += (_, _) => SetAppActions();
        }

        private void SetAppActions()
        {
            AppActionsHelper.SetAppActions(_i18n);
        }

        private void SetTitle()
        {
            if (Windows.Count > 0)
            {
                Windows[0].Title = _i18n.T("Swashbuckler Diary");
            }
        }
    }
}