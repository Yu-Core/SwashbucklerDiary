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
            var window = new Window(new MainPage(backgroundColor, _navigateController));
            window.Resumed += (s, e) => _appLifecycle.OnResume();
            window.Stopped += (s, e) => _appLifecycle.OnStop();
            window.Created += WindowCreated;
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
            if (_themeService is ThemeService themeService)
            {
                themeService.SetTheme(theme);
            }

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
            AppActionsHelper.AddAppActions(_i18n);
            _i18n.OnChanged += _ =>
            {
                AppActionsHelper.AddAppActions(_i18n);
            };
        }
    }
}