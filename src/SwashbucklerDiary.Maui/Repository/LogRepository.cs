using SqlSugar;
using SwashbucklerDiary.Maui.IRepository;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Repository
{
    public class LogRepository : BaseRepository<LogModel>, ILogRepository
    {
        public LogRepository(ISqlSugarClient context) : base(context)
        {
        }
    }
}
