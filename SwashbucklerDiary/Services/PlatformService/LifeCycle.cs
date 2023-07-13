namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        public event Action? Resumed;
        public event Action? Stopped;

        public void OnResume()
        {
            Resumed?.Invoke();
        }

        public void OnStop()
        {
            Stopped?.Invoke();
        }

        public void QuitApp()
        {
            App.Current!.Quit();
        }
    }
}
