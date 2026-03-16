using SqlSugar;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Repository
{
    public class LogRepository : BaseRepository<LogModel>, ILogRepository
    {
        public LogRepository(ISqlSugarClient context) : base(context)
        {
        }

        public override ISqlSugarClient Context
        {
            get
            {
                if (base.Context is SqlSugarScope)
                {
                    return base.Context.AsTenant().GetConnection(SQLiteConstants.LogDatabaseFilename);
                }
                else
                {
                    return base.Context.CopyNew().AsTenant().GetConnection(SQLiteConstants.LogDatabaseFilename);
                }
            }
            set => base.Context = value;
        }
    }
}
