namespace SwashbucklerDiary.Rcl.Services
{
    public interface IVersionUpdataManager
    {
        public event Action? AfterVersionUpdate;

        Task HandleVersionUpdate();

        Task<bool> CheckForUpdates();

        Task ToUpdate();
    }
}
