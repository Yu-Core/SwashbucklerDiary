namespace SwashbucklerDiary.Rcl.Essentials
{
    public interface IAppLifecycle
    {
        ActivationArguments? ActivationArguments { get; set; }

        event Action<ActivationArguments>? OnActivated;

        event Action OnResumed;

        event Action OnStopped;

        void Resume();

        void Stop();

        /// <summary>
        /// 退出应用
        /// </summary>
        void QuitApp();
    }
}
