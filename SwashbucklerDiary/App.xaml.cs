using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary
{
    public partial class App : Application
    {
        private IPlatformService PlatformService { get; set; }
        public App(IPlatformService platformService)
        {
            InitializeComponent();

            MainPage = new MainPage();
            
            PlatformService = platformService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = base.CreateWindow(activationState);
            window.Resumed += (s, e) => PlatformService.OnResume();
            window.Stopped += (s, e) => PlatformService.OnStop();
            return window;
        }
    }
}