namespace SwashbucklerDiary.Rcl.Services
{
    public interface IWebDavDiarySyncScheduler
    {
        void Start();

        Task<DiarySyncResult?> RunOnceAsync(bool force = false);
    }
}
