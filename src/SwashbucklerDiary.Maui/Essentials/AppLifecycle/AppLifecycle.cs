using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui.Essentials
{
    public class AppLifecycle : IAppLifecycle
    {
        public event Action? Resumed;

        public event Action? Stopped;

        public void OnResume() => Resumed?.Invoke();

        public void OnStop() => Stopped?.Invoke();

        public void QuitApp()
        {
            Application.Current!.Quit();
        }
    }
}
