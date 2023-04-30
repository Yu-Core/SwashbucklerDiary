using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary
{
    public partial class App : Application
    {
        private ISystemService SystemService { get; set; }
        public App(ISystemService systemService)
        {
            InitializeComponent();

            MainPage = new MainPage();
            SystemService = systemService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = base.CreateWindow(activationState);

#if MACCATALYST
            window.Created += MacTitleBar.InitTitleBarForMac;
#endif
            window.Resumed += (s, e) => SystemService.OnResume();
            window.Stopped += (s, e) => SystemService.OnStop();
            return window;
        }
    }
}