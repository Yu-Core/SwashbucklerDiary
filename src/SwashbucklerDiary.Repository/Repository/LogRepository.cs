using SqlSugar;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Repository
{
    public class LogRepository : BaseRepository<LogModel>, ILogRepository
    {
        public LogRepository(ISqlSugarClient context) : base(context)
        {
        }
    }
}
