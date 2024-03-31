using SwashbucklerDiary.Rcl;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui
{
    public partial class App : Application
    {
        private readonly IAppLifecycle _appLifecycle;

        public App(IAppLifecycle appLifecycle)
        {
            InitializeComponent();

            this.Resources["PageBackgroundColor"] = Color.FromArgb(ThemeColor.LightSurface);

            MainPage = new MainPage();

            _appLifecycle = appLifecycle;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = base.CreateWindow(activationState);
            window.Resumed += (s, e) => _appLifecycle.OnResume();
            window.Stopped += (s, e) => _appLifecycle.OnStop();
            return window;
        }
    }
}