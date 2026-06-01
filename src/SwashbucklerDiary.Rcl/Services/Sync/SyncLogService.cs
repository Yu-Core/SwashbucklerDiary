using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public class SyncLogService : BaseDataService<SyncLogModel>, ISyncLogService
    {
        private readonly ISyncLogRepository _syncLogRepository;

        public SyncLogService(ISyncLogRepository syncLogRepository)
        {
            base._iBaseRepository = syncLogRepository;
            _syncLogRepository = syncLogRepository;
        }

        public async Task AddLogAsync(SyncLogModel log)
        {
            try
            {
                EnsureTable();
                DateTime now = DateTime.Now;
                log.Id = Guid.NewGuid();
                log.CreateTime = now;
                log.UpdateTime = now;
                await AddAsync(log).ConfigureAwait(false);
            }
            catch
            {
                // Sync logs are diagnostic only and must not break sync/backup flows.
            }
        }

        public async Task<List<SyncLogModel>> QueryLatestAsync(int count = 20)
        {
            EnsureTable();
            var logs = await QueryAsync().ConfigureAwait(false);
            return logs
                .OrderByDescending(it => it.CreateTime)
                .Take(count)
                .ToList();
        }

        private void EnsureTable()
        {
            _syncLogRepository.EnsureTable();
        }
    }
}
