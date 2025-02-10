using SwashbucklerDiary.Rcl.Essentials;
using Application = Gtk.Application;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public class AppLifecycle : IAppLifecycle
    {
        public ActivationArguments? ActivationArguments
        {
            get => AppActivation.Arguments;
            set => AppActivation.Arguments = value;
        }

        public event Action<ActivationArguments>? OnActivated
        {
            add => AppActivation.OnActivated += value;
            remove => AppActivation.OnActivated -= value;
        }

        public event Action? OnResumed;

        public event Action? OnStopped;

        public void Resume() => OnResumed?.Invoke();

        public void Stop() => OnStopped?.Invoke();

        public void QuitApp()
        {
            Application.GetDefault()?.Quit();
        }
    }
}
