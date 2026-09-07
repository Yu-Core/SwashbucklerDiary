namespace SwashbucklerDiary.Rcl.Services
{
    public class NoopWebDavDiarySyncScheduler : IWebDavDiarySyncScheduler
    {
        public void Start()
        {
        }

        public Task<DiarySyncResult?> RunOnceAsync(bool force = false)
        {
            return Task.FromResult<DiarySyncResult?>(null);
        }
    }
}
