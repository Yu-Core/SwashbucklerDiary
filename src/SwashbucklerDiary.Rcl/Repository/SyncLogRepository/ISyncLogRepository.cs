using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Repository
{
    public interface ISyncLogRepository : IBaseRepository<SyncLogModel>
    {
        void EnsureTable();
    }
}
