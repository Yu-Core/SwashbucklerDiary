using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public class LogService : BaseDataService<LogModel>, ILogService
    {
        private readonly ILogRepository _iLogRepository;

        public LogService(ILogRepository iLogRepository)
        {
            base._iBaseRepository = iLogRepository;
            _iLogRepository = iLogRepository;
        }
    }
}
