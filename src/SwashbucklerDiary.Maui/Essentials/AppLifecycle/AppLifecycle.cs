using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui.Essentials
{
    public class AppLifecycle : IAppLifecycle, IDisposable
    {
        public ActivationArguments? ActivationArguments
        {
            get => LaunchActivation.ActivationArguments;
            set => LaunchActivation.ActivationArguments = value;
        }

        public event Action<ActivationArguments>? Activated;

        public event Action? Resumed;

        public event Action? Stopped;

        public AppLifecycle()
        {
            LaunchActivation.Activated += OnActivate;
        }

        public void OnResume() => Resumed?.Invoke();

        public void OnStop() => Stopped?.Invoke();

        public void QuitApp()
        {
            Application.Current!.Quit();
        }

        private void OnActivate(ActivationArguments args) => Activated?.Invoke(args);

        public void Dispose()
        {
            LaunchActivation.Activated -= OnActivate;
            GC.SuppressFinalize(this);
        }
    }
}
