using SqlSugar;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Repository
{
    public class LogRepository : BaseRepository<LogModel>, ILogRepository
    {
        public LogRepository(ISqlSugarClient context,
            ISettingService settingService) : base(context, settingService)
        {
        }

        public override ISqlSugarClient Context
        {
            get => Itenant.GetConnection(SQLiteConstants.LogDatabaseFilename);
            set => base.Context = value;
        }
    }
}
