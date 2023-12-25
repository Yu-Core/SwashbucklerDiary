using SwashbucklerDiary.Maui.IRepository;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Services
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
