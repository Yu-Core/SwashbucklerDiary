using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Pages;
using SwashbucklerDiary.Shared;
using System.Text.Json;

namespace SwashbucklerDiary.Rcl.Services
{
    public class WebDavDiarySyncScheduler : IWebDavDiarySyncScheduler, IDisposable
    {
        private readonly IDiarySyncService _diarySyncService;
        private readonly IWebDAV _webDAV;
        private readonly ISettingService _settingService;
        private readonly ISyncLogService _syncLogService;
        private readonly ILogger<WebDavDiarySyncScheduler> _logger;
        private readonly SemaphoreSlim _syncLock = new(1, 1);

        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _worker;

        public WebDavDiarySyncScheduler(IDiarySyncService diarySyncService,
            IWebDAV webDAV,
            ISettingService settingService,
            ISyncLogService syncLogService,
            ILogger<WebDavDiarySyncScheduler> logger)
        {
            _diarySyncService = diarySyncService;
            _webDAV = webDAV;
            _settingService = settingService;
            _syncLogService = syncLogService;
            _logger = logger;
        }

        public void Start()
        {
            if (_worker is not null)
            {
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _worker = Task.Run(() => RunLoopAsync(_cancellationTokenSource.Token));
        }

        public async Task<DiarySyncResult?> RunOnceAsync(bool force = false)
        {
            if (!force && !ShouldAutoSync())
            {
                return null;
            }

            if (!_syncLock.Wait(0))
            {
                return null;
            }

            try
            {
                if (!await EnsureWebDavAsync().ConfigureAwait(false))
                {
                    return null;
                }

                var result = await _diarySyncService.SyncAsync().ConfigureAwait(false);
                await _settingService.SetAsync(s => s.WebDAVDiarySyncLastSyncTime, DateTime.Now).ConfigureAwait(false);
                await _syncLogService.AddLogAsync(new SyncLogModel()
                {
                    Operation = force ? "Manual sync" : "Auto sync",
                    Success = true,
                    Pushed = result.Pushed,
                    Pulled = result.Pulled,
                    Deleted = result.Deleted,
                    Conflicts = result.Conflicts
                }).ConfigureAwait(false);
                return result;
            }
            catch (Exception e)
            {
                await _syncLogService.AddLogAsync(new SyncLogModel()
                {
                    Operation = force ? "Manual sync" : "Auto sync",
                    Success = false,
                    Message = e.Message
                }).ConfigureAwait(false);
                _logger.LogError(e, "WebDAV diary auto sync failed");
                return null;
            }
            finally
            {
                _syncLock.Release();
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _syncLock.Dispose();
            GC.SuppressFinalize(this);
        }

        private async Task RunLoopAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken).ConfigureAwait(false);
            while (!cancellationToken.IsCancellationRequested)
            {
                await RunOnceAsync().ConfigureAwait(false);
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken).ConfigureAwait(false);
            }
        }

        private bool ShouldAutoSync()
        {
            if (!_settingService.Get(s => s.WebDAVDiarySyncAuto))
            {
                return false;
            }

            if (_settingService.GetTemp(it => it.PrivacyMode))
            {
                return false;
            }

            int intervalMinutes = Math.Max(5, _settingService.Get(s => s.WebDAVDiarySyncIntervalMinutes, 30));
            DateTime lastSyncTime = _settingService.Get(s => s.WebDAVDiarySyncLastSyncTime, DateTime.MinValue);
            return lastSyncTime == DateTime.MinValue
                || (DateTime.Now - lastSyncTime).TotalMinutes >= intervalMinutes;
        }

        private async Task<bool> EnsureWebDavAsync()
        {
            if (_webDAV.Initialized)
            {
                return true;
            }

            string configJson = _settingService.Get(s => s.WebDavConfig);
            if (string.IsNullOrWhiteSpace(configJson))
            {
                return false;
            }

            var config = JsonSerializer.Deserialize<WebDavConfigForm>(configJson);
            if (config is null)
            {
                return false;
            }

            await _webDAV.Set(config.ServerAddress, config.Account, config.Password).ConfigureAwait(false);
            return true;
        }
    }
}
