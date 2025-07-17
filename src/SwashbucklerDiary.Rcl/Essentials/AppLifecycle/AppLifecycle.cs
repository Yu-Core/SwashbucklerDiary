
namespace SwashbucklerDiary.Rcl.Essentials
{
    public abstract class AppLifecycle : IAppLifecycle
    {
        public bool IsLaunched => OnActivated is not null;
        public ActivationArguments? ActivationArguments { get; set; }

        public event Action<ActivationArguments>? OnActivated;
        public event Action? OnResumed;
        public event Action? OnStopped;

        public virtual void Activate(ActivationArguments arguments)
            => OnActivated?.Invoke(arguments);
        public virtual void Resume()
            => OnResumed?.Invoke();
        public virtual void Stop()
            => OnStopped?.Invoke();

        public abstract void QuitApp();
    }
}
