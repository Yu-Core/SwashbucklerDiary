using SqlSugar;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Repository
{
    public class SyncLogRepository : BaseRepository<SyncLogModel>, ISyncLogRepository
    {
        public SyncLogRepository(ISqlSugarClient context, ISettingService settingService)
            : base(context, settingService)
        {
        }

        public void EnsureTable()
        {
            Context.CodeFirst.InitTables<SyncLogModel>();
        }
    }
}
