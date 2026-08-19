using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface ISyncLogService : IBaseDataService<SyncLogModel>
    {
        Task AddLogAsync(SyncLogModel log);

        Task<List<SyncLogModel>> QueryLatestAsync(int count = 20);
    }
}
