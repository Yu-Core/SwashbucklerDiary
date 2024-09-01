using SwashbucklerDiary.Maui.Essentials;
using SwashbucklerDiary.Rcl;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui
{
    public partial class App : Application
    {
        private readonly IAppLifecycle _appLifecycle;

        private readonly IThemeService _themeService;

        private readonly Masa.Blazor.MasaBlazor _masaBlazor;

        private Color backgroundColor = default!;

        public App(IAppLifecycle appLifecycle,
            IThemeService themeService,
            Masa.Blazor.MasaBlazor masaBlazor)
        {
            InitializeComponent();

            _themeService = themeService;
            _masaBlazor = masaBlazor;
            _appLifecycle = appLifecycle;
            InitTheme();

            MainPage = new MainPage(backgroundColor);

        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = base.CreateWindow(activationState);
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
            ((ThemeService)_themeService).SetTheme(theme);
            _themeService.OnChanged += ThemeChanged;

            bool dark = _themeService.RealTheme == Shared.Theme.Dark;
            backgroundColor = Color.FromArgb(dark ? ThemeColor.DarkSurface : ThemeColor.LightSurface);
        }

        private void ThemeChanged(Theme theme)
        {
            _masaBlazor.SetTheme(theme == Theme.Dark);
            TitleBarOrStatusBar.SetTitleBarOrStatusBar(theme);
        }
    }
}