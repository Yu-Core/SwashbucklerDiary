namespace SwashbucklerDiary.Rcl.Services
{
    public interface IWebDavBackupScheduler
    {
        void Start();

        Task<bool> RunOnceAsync(bool force = false);
    }
}
