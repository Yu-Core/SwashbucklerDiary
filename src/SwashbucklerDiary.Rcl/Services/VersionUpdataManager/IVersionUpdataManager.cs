namespace SwashbucklerDiary.Rcl.Services
{
    public interface IVersionUpdataManager
    {
        public event Action? AfterVersionUpdate;

        public event Action? OnCheckUpdate;

        Task HandleVersionUpdate();

        void CheckUpdates();

        Task<Release?> GetLastReleaseAsync();
    }
}
