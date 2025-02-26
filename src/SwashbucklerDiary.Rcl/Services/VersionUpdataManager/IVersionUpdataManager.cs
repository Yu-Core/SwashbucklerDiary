namespace SwashbucklerDiary.Rcl.Services
{
    public interface IVersionUpdataManager
    {
        public event Action? AfterFirstEnter;

        public event Action? AfterVersionUpdate;

        public event Action? AfterCheckFirstLaunch;

        void NotifyAfterFirstEnter();

        Task HandleVersionUpdate();

        void NotifyAfterCheckFirstLaunch();

        Task<bool> CheckForUpdates();

        Task ToUpdate();
    }
}
