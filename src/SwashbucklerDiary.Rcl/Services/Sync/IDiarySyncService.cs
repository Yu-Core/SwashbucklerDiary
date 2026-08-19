namespace SwashbucklerDiary.Rcl.Services
{
    public interface IDiarySyncService
    {
        Task<DiarySyncResult> SyncAsync();

        Task<bool> HasLocalChangesAsync();
    }
}
