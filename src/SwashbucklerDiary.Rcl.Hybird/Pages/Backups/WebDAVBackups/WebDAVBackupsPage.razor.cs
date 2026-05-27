using DeepCloner.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using System.Text.Json;

namespace SwashbucklerDiary.Rcl.Pages
{
    // TODO: 写的不好，暂时先将就着，以后需要重写
    public partial class WebDAVBackupsPage : ImportantComponentBase
    {
        private bool showConfig;

        private bool showUpload;

        private bool showDownload;

        private bool includeDiaryResources;

        private bool autoSync;

        private int autoSyncIntervalMinutes;

        private WebDavConfigForm configModel = new();

        private const string webDavFolderName = "SwashbucklerDiary";

        private List<WebDavIncrementalBackupManifest> backupManifests = [];

        [Inject]
        private IWebDAV WebDAVService { get; set; } = default!;

        [Inject]
        private ILogger<WebDAVBackupsPage> Logger { get; set; } = default!;

        [Inject]
        private IDiaryFileManager DiaryFileManager { get; set; } = default!;

        [Inject]
        private IDiarySyncService DiarySyncService { get; set; } = default!;

        [Inject]
        private IWebDavDiarySyncScheduler DiarySyncScheduler { get; set; } = default!;

        [Inject]
        private IWebDavIncrementalBackupService IncrementalBackupService { get; set; } = default!;

        protected override void ReadSettings()
        {
            var configJson = SettingService.Get(s => s.WebDavConfig);
            if (!string.IsNullOrEmpty(configJson))
            {
                configModel = JsonSerializer.Deserialize<WebDavConfigForm>(configJson) ?? new();
            }

            includeDiaryResources = SettingService.Get(s => s.WebDAVCopyResources);
            autoSync = SettingService.Get(s => s.WebDAVDiarySyncAuto);
            autoSyncIntervalMinutes = SettingService.Get(s => s.WebDAVDiarySyncIntervalMinutes, 30);
        }

        private bool Configured => !string.IsNullOrEmpty(configModel.ServerAddress);

        private string ConfiguredText => Configured ? I18n.T("Configured") : I18n.T("Not configured");

        private string AutoSyncIntervalText => $"{autoSyncIntervalMinutes} {I18n.T("Minute")}";

        private string GeFileSize(WebDavIncrementalBackupManifest manifest)
            => AppFileSystem.ConvertBytesToReadable(manifest.LegacyLength ?? manifest.Files.Sum(it => it.Length));

        private int GetFileCount(WebDavIncrementalBackupManifest manifest)
            => manifest.LegacyZip ? 1 : manifest.Files.Count;

        private string GetBackupIcon(WebDavIncrementalBackupManifest manifest)
            => manifest.LegacyZip ? "mdi:mdi-folder-zip-outline" : "mdi:mdi-database-sync-outline";

        private string GetBackupTime(WebDavIncrementalBackupManifest manifest)
            => manifest.CreatedAt.ToString("yyyy-MM-dd HH:mm");

        private async Task SaveWebDavConfig(WebDavConfigForm webDavConfig)
        {
            bool isSuccess = await SetWebDav(webDavConfig);
            if (isSuccess)
            {
                configModel = webDavConfig.DeepClone();
                showConfig = false;
                await AlertService.SuccessAsync(I18n.T("Configuration successful"));
                var configJson = JsonSerializer.Serialize(configModel);
                await SettingService.SetAsync(s => s.WebDavConfig, configJson);
            }
        }

        private async Task<bool> SetWebDav(WebDavConfigForm webDavConfig)
        {
            AlertService.StartLoading();

            try
            {
                await WebDAVService.Set(webDavConfig.ServerAddress, webDavConfig.Account, webDavConfig.Password);
                return true;
            }
            catch (ArgumentException e)
            {
                await AlertService.ErrorAsync(I18n.T("Configuration failed, please check the configuration information"));
                Logger.LogError(e, $"SaveWebDavConfig {nameof(ArgumentException)}");
            }
            catch (HttpRequestException e)
            {
                await AlertService.ErrorAsync(I18n.T("Network error"));
                Logger.LogError(e, $"SaveWebDavConfig {nameof(HttpRequestException)}");
            }
            catch (WebDAVException e)
            {
                await AlertService.ErrorAsync(I18n.T("Configuration failed, please check the configuration information"));
                Logger.LogError(e, $"SaveWebDavConfig {nameof(WebDAVException)}");
            }
            catch (Exception e)
            {
                await AlertService.ErrorAsync(I18n.T("Configuration failed for unknown reasons"));
                Logger.LogError(e, "SaveWebDavConfig Unknown");
            }
            finally
            {
                AlertService.StopLoading();
            }

            return false;
        }

        private async Task OpenUploadDialog()
        {
            AlertService.StartLoading();

            try
            {
                var flag = await Check();
                if (!flag)
                {
                    return;
                }

                showUpload = true;
            }
            finally
            {
                AlertService.StopLoading();
            }
        }

        private async Task Upload()
        {
            showUpload = false;

            AlertService.StartLoading();

            try
            {
                try
                {
                    var manifest = await IncrementalBackupService.UploadAsync(includeDiaryResources);
                    await AlertService.SuccessAsync($"{I18n.T("Incremental backup successfully")}\n{I18n.T("File")}: {manifest.Files.Count}");
                }
                catch (HttpRequestException e)
                {
                    await AlertService.ErrorAsync(I18n.T("Network error"));
                    Logger.LogError(e, $"OpenDownloadDialog {nameof(HttpRequestException)}");
                }
                catch (Exception e)
                {
                    await AlertService.ErrorAsync($"{I18n.T("Upload failed")}\n{e}");
                    Logger.LogError(e, $"Backups Upload Fail");
                }
            }
            finally
            {
                AlertService.StopLoading();
            }
        }

        private async Task OpenDownloadDialog()
        {
            AlertService.StartLoading();
            try
            {
                var flag = await Check();
                if (!flag)
                {
                    return;
                }

                showDownload = true;
                StateHasChanged();
                try
                {
                    backupManifests = await IncrementalBackupService.GetManifestsAsync();
                }
                catch (HttpRequestException e)
                {
                    await AlertService.ErrorAsync(I18n.T("Network error"));
                    Logger.LogError(e, $"OpenDownloadDialog {nameof(HttpRequestException)}");
                }
                catch (Exception e)
                {
                    await AlertService.ErrorAsync($"{I18n.T("Pull failed")}\n{e}");
                    Logger.LogError(e, $"Backups Download Fail");
                }
            }
            finally
            {
                AlertService.StopLoading();
            }
        }

        private async Task Download(WebDavIncrementalBackupManifest manifest)
        {
            showDownload = false;

            AlertService.StartLoading();
            try
            {
                if (await DiarySyncService.HasLocalChangesAsync())
                {
                    await AlertService.ErrorAsync(I18n.T("Local diary changes are not synced. Sync diaries before pulling backup."));
                    return;
                }

                await IncrementalBackupService.RestoreAsync(manifest);
                await AlertService.SuccessAsync(I18n.T("Pull successfully"));
            }
            catch (HttpRequestException e)
            {
                await AlertService.ErrorAsync(I18n.T("Network error"));
                Logger.LogError(e, $"OpenDownloadDialog {nameof(HttpRequestException)}");
            }
            catch (Exception e)
            {
                await AlertService.ErrorAsync($"{I18n.T("Pull failed")}\n{e}");
                Logger.LogError(e, "WebDAV Download fail");
            }
            finally
            {
                AlertService.StopLoading();
            }
        }

        private async Task SyncDiaries()
        {
            AlertService.StartLoading();

            try
            {
                var flag = await Check();
                if (!flag)
                {
                    return;
                }

                var result = await DiarySyncScheduler.RunOnceAsync(true) ?? await DiarySyncService.SyncAsync();
                await SettingService.SetAsync(s => s.WebDAVDiarySyncLastSyncTime, DateTime.Now);
                await AlertService.SuccessAsync($"{I18n.T("Sync successfully")}\n{I18n.T("Upload")}: {result.Pushed}, {I18n.T("Pull")}: {result.Pulled}, {I18n.T("Delete")}: {result.Deleted}, {I18n.T("Conflict")}: {result.Conflicts}");
            }
            catch (HttpRequestException e)
            {
                await AlertService.ErrorAsync(I18n.T("Network error"));
                Logger.LogError(e, $"SyncDiaries {nameof(HttpRequestException)}");
            }
            catch (Exception e)
            {
                await AlertService.ErrorAsync($"{I18n.T("Sync failed")}\n{e}");
                Logger.LogError(e, "SyncDiaries fail");
            }
            finally
            {
                AlertService.StopLoading();
            }
        }

        private async Task ToggleAutoSyncInterval()
        {
            autoSyncIntervalMinutes = autoSyncIntervalMinutes switch
            {
                < 30 => 30,
                < 60 => 60,
                _ => 15
            };

            await SettingService.SetAsync(s => s.WebDAVDiarySyncIntervalMinutes, autoSyncIntervalMinutes);
        }

        private async Task<bool> Check()
        {
            if (!Configured)
            {
                await AlertService.ErrorAsync(I18n.T("Configure WebDAV first"));
                return false;
            }

            if (!WebDAVService.Initialized)
            {
                return await SetWebDav(configModel);
            }

            return true;
        }
    }
}
