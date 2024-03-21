namespace SwashbucklerDiary.Rcl.Services
{
    public interface IVersionUpdataManager
    {
        public event Func<Task>? AfterFirstEnter;

        public event Func<Task>? AfterVersionUpdate;

        public event Func<Task>? AfterCheckFirstLaunch;

        Task NotifyAfterFirstEnter();

        Task HandleVersionUpdate();

        Task NotifyAfterCheckFirstLaunch();

        Task<bool> CheckForUpdates();

        Task ToUpdate();
    }
}
