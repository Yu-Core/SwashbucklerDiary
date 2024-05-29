namespace SwashbucklerDiary.Rcl.Essentials
{
    public interface IAppLifecycle
    {
        ActivationArguments? ActivationArguments { get; set; }

        event Action<ActivationArguments>? Activated;

        event Action Resumed;

        event Action Stopped;

        void OnResume();

        void OnStop();

        /// <summary>
        /// 退出应用
        /// </summary>
        void QuitApp();
    }
}
