using SqlSugar;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Repository
{
    public class LogRepository : BaseRepository<LogModel>, ILogRepository
    {
        public LogRepository(ISqlSugarClient context) : base(context)
        {
        }
    }
}
