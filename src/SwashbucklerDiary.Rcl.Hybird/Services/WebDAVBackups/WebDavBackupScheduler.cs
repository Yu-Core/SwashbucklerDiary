using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Pages;
using System.Text.Json;

namespace SwashbucklerDiary.Rcl.Services
{
    public class WebDavBackupScheduler : IWebDavBackupScheduler, IDisposable
    {
        private readonly IWebDavIncrementalBackupService _backupService;
        private readonly IWebDAV _webDAV;
        private readonly ISettingService _settingService;
        private readonly ILogger<WebDavBackupScheduler> _logger;
        private readonly SemaphoreSlim _backupLock = new(1, 1);

        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _worker;

        public WebDavBackupScheduler(IWebDavIncrementalBackupService backupService,
            IWebDAV webDAV,
            ISettingService settingService,
            ILogger<WebDavBackupScheduler> logger)
        {
            _backupService = backupService;
            _webDAV = webDAV;
            _settingService = settingService;
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

        public async Task<bool> RunOnceAsync(bool force = false)
        {
            if (!force && !ShouldAutoBackup())
            {
                return false;
            }

            if (!_backupLock.Wait(0))
            {
                return false;
            }

            try
            {
                if (!await EnsureWebDavAsync().ConfigureAwait(false))
                {
                    return false;
                }

                bool includeDiaryResources = _settingService.Get(s => s.WebDAVCopyResources);
                await _backupService.UploadAsync(includeDiaryResources).ConfigureAwait(false);
                await _settingService.SetAsync(s => s.WebDAVBackupLastBackupTime, DateTime.Now).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "WebDAV auto backup failed");
                return false;
            }
            finally
            {
                _backupLock.Release();
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _backupLock.Dispose();
            GC.SuppressFinalize(this);
        }

        private async Task RunLoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(20), cancellationToken).ConfigureAwait(false);
                while (!cancellationToken.IsCancellationRequested)
                {
                    await RunOnceAsync().ConfigureAwait(false);
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private bool ShouldAutoBackup()
        {
            if (!_settingService.Get(s => s.WebDAVBackupAuto))
            {
                return false;
            }

            if (_settingService.GetTemp(it => it.PrivacyMode))
            {
                return false;
            }

            int intervalHours = Math.Max(1, _settingService.Get(s => s.WebDAVBackupIntervalHours, 24));
            DateTime lastBackupTime = _settingService.Get(s => s.WebDAVBackupLastBackupTime, DateTime.MinValue);
            return lastBackupTime == DateTime.MinValue
                || (DateTime.Now - lastBackupTime).TotalHours >= intervalHours;
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
