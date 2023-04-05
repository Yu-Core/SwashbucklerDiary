using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Services
{
    public class LogService : BaseService<LogModel>, ILogService
    {
        private readonly ILogRepository _iLogRepository;

        public LogService(ILogRepository iLogRepository)
        {
            base._iBaseRepository = iLogRepository;
            _iLogRepository = iLogRepository;
        }
    }
}
