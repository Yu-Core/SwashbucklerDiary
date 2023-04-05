using SqlSugar;
using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Repository
{
    public class LogRepository : BaseRepository<LogModel>, ILogRepository
    {
        public LogRepository(ISqlSugarClient context) : base(context)
        {
        }
    }
}
