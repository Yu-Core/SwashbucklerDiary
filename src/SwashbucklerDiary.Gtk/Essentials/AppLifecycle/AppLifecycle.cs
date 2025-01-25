using SwashbucklerDiary.Rcl.Essentials;
using Application = Gtk.Application;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public class AppLifecycle : IAppLifecycle
    {
        public ActivationArguments? ActivationArguments
        {
            get => LaunchActivation.ActivationArguments;
            set => LaunchActivation.ActivationArguments = value;
        }

        public event Action<ActivationArguments>? Activated
        {
            add => LaunchActivation.Activated += value;
            remove => LaunchActivation.Activated -= value;
        }

        public event Action? Resumed;

        public event Action? Stopped;

        public void OnResume() => Resumed?.Invoke();

        public void OnStop() => Stopped?.Invoke();

        public void QuitApp()
        {
            Application.Quit();
        }
    }
}
