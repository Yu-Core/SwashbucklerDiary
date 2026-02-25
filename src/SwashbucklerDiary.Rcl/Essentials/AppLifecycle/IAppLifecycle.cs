namespace SwashbucklerDiary.Rcl.Essentials
{
    public interface IAppLifecycle
    {
        bool IsLaunched { get; }

        ActivationArguments? ActivationArguments { get; set; }

        event Action? AfterFirstEntered;

        event Action<ActivationArguments>? Activated;

        event Action? Resumed;

        event Action? Stopped;

        void NotifyActivated(ActivationArguments arguments);

        void NotifyAfterFirstEntered();

        void NotifyResumed();

        void NotifyStopped();

        /// <summary>
        /// 退出应用
        /// </summary>
        void QuitApp();
    }
}
