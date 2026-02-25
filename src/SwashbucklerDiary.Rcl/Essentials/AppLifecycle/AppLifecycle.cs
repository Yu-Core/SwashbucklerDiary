
namespace SwashbucklerDiary.Rcl.Essentials
{
    public abstract class AppLifecycle : IAppLifecycle
    {
        public bool IsLaunched => Activated is not null;
        public ActivationArguments? ActivationArguments { get; set; }

        public event Action<ActivationArguments>? Activated;
        public event Action? Resumed;
        public event Action? Stopped;
        public event Action? AfterFirstEntered;

        public void NotifyActivated(ActivationArguments arguments)
            => Activated?.Invoke(arguments);
        public void NotifyResumed()
            => Resumed?.Invoke();
        public void NotifyStopped()
            => Stopped?.Invoke();
        public void NotifyAfterFirstEntered()
            => AfterFirstEntered?.Invoke();
        public abstract void QuitApp();

    }
}
