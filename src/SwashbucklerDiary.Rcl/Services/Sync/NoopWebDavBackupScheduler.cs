namespace SwashbucklerDiary.Rcl.Services
{
    public class NoopWebDavBackupScheduler : IWebDavBackupScheduler
    {
        public void Start()
        {
        }

        public Task<bool> RunOnceAsync(bool force = false)
        {
            return Task.FromResult(false);
        }
    }
}
