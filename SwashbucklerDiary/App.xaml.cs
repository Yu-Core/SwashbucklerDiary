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

        protected override void OnResume()
        {
            base.OnResume();
            SystemService.OnResume();
        }
    }
}