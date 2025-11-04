namespace SwashbucklerDiary.Rcl.Services
{
    public interface IVersionUpdataManager
    {
        public event Action? AfterVersionUpdate;

        Task HandleVersionUpdate();

        Task<Release?> GetLastReleaseAsync();

        Task ToUpdate();
    }
}
