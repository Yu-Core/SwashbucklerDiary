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

        private readonly IServiceProvider _serviceProvider;

        private readonly IAppLifecycle _appLifecycle;

        private readonly IThemeService _themeService;

        private readonly Masa.Blazor.MasaBlazor _masaBlazor;

        private readonly II18nService _i18n;

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            _serviceProvider = serviceProvider;
            _themeService = serviceProvider.GetRequiredService<IThemeService>();
            _masaBlazor = serviceProvider.GetRequiredService<Masa.Blazor.MasaBlazor>();
            _appLifecycle = serviceProvider.GetRequiredService<IAppLifecycle>();
            _i18n = serviceProvider.GetRequiredService<II18nService>();
            InitTheme();
            InitAppActions();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new MainPage(_serviceProvider, backgroundColor));
            window.Resumed += (s, e) => _appLifecycle.NotifyResumed();
            window.Stopped += (s, e) => _appLifecycle.NotifyStopped();
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

            _themeService.ThemeChanged += ThemeChanged;

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

#if WINDOWS
            TitleBarButtonDirectionHelper.UpdateTitleBarButtonDirection(Windows[0], _i18n.Culture.TextInfo.IsRightToLeft);
#endif
        }
    }
}