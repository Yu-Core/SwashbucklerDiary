namespace SwashbucklerDiary.Rcl.Web.Essentials
{
    public class AppLifecycle : Rcl.Essentials.AppLifecycle, IDisposable
    {
        private readonly AppLifecycleJSModule _jSModule;

        public AppLifecycle(AppLifecycleJSModule jSModule)
        {
            _jSModule = jSModule;
            _jSModule.Resumed += NotifyResumed;
            _jSModule.Stopped += NotifyStopped;
        }

        public override async void QuitApp()
        {
            await _jSModule.Quit();
        }

        public async Task InitializedAsync()
        {
            await _jSModule.Init();
        }

        public void Dispose()
        {
            _jSModule.Resumed -= NotifyResumed;
            _jSModule.Stopped -= NotifyStopped;
            GC.SuppressFinalize(this);
        }
    }
}
