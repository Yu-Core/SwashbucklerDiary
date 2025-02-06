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

        private WebDavConfigForm configModel = new();

        private const string webDavFolderName = "SwashbucklerDiary";

        private List<string> fileList = [];

        [Inject]
        private IWebDAV WebDAVService { get; set; } = default!;

        [Inject]
        private ILogger<WebDAVBackupsPage> Logger { get; set; } = default!;

        [Inject]
        private IDiaryFileManager DiaryFileManager { get; set; } = default!;

        protected override void ReadSettings()
        {
            var configJson = SettingService.Get(s => s.WebDavConfig);
            if (!string.IsNullOrEmpty(configJson))
            {
                configModel = JsonSerializer.Deserialize<WebDavConfigForm>(configJson) ?? new();
            }

            includeDiaryResources = SettingService.Get(s => s.WebDAVCopyResources);
        }

        private bool Configured => !string.IsNullOrEmpty(configModel.ServerAddress);

        private string ConfiguredText => Configured ? I18n.T("Backups.Config.Configured") : I18n.T("Backups.Config.NotConfigured");

        private async Task SaveWebDavConfig(WebDavConfigForm webDavConfig)
        {
            bool isSuccess = await SetWebDav(webDavConfig);
            if (isSuccess)
            {
                configModel = webDavConfig.DeepClone();
                showConfig = false;
                await PopupServiceHelper.Success(I18n.T("Backups.Config.Success"));
                var configJson = JsonSerializer.Serialize(configModel);
                await SettingService.SetAsync(s => s.WebDavConfig, configJson);
            }
        }

        private async Task<bool> SetWebDav(WebDavConfigForm webDavConfig)
        {
            try
            {
                await WebDAVService.Set(webDavConfig.ServerAddress, webDavConfig.Account, webDavConfig.Password);
                return true;
            }
            catch (ArgumentException e)
            {
                await PopupServiceHelper.Error(I18n.T("Backups.Config.Fail.ConfigInfo"));
                Logger.LogError(e, $"SaveWebDavConfig {nameof(ArgumentException)}");
            }
            catch (HttpRequestException e)
            {
                await PopupServiceHelper.Error(I18n.T("Backups.Config.Fail.Network"));
                Logger.LogError(e, $"SaveWebDavConfig {nameof(HttpRequestException)}");
            }
            catch (WebDAVException e)
            {
                await PopupServiceHelper.Error(I18n.T("Backups.Config.Fail.ConfigInfo"));
                Logger.LogError(e, $"SaveWebDavConfig {nameof(WebDAVException)}");
            }
            catch (Exception e)
            {
                await PopupServiceHelper.Error(I18n.T("Backups.Config.Fail.Unknown"));
                Logger.LogError(e, "SaveWebDavConfig Unknown");
            }

            return false;
        }

        private async Task OpenUploadDialog()
        {
            var flag = await Check();
            if (!flag)
            {
                return;
            }

            showUpload = true;
        }

        private async Task Upload()
        {
            showUpload = false;

            await PopupServiceHelper.StartLoading();

            string filePath = await DiaryFileManager.ExportDBAsync(includeDiaryResources);
            using var stream = File.OpenRead(filePath);
            string fileName = DiaryFileManager.GetBackupFileName();
            string destFileName = webDavFolderName + "/" + fileName;
            try
            {
                await WebDAVService.UploadAsync(destFileName, stream);
                await PopupServiceHelper.Success(I18n.T("Backups.Upload.Success"));
            }
            catch (HttpRequestException e)
            {
                await PopupServiceHelper.Error(I18n.T("Backups.Config.Fail.Network"));
                Logger.LogError(e, $"OpenDownloadDialog {nameof(HttpRequestException)}");
            }
            catch (Exception e)
            {
                await PopupServiceHelper.Error(I18n.T("Backups.Upload.Fail"));
                Logger.LogError(e, $"Backups Upload Fail");
            }
            finally
            {
                await PopupServiceHelper.StopLoading();
            }
        }

        private async Task OpenDownloadDialog()
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
                fileList = await WebDAVService.GetZipFileListAsync(webDavFolderName);
            }
            catch (HttpRequestException e)
            {
                await PopupServiceHelper.Error(I18n.T("Backups.Config.Fail.Network"));
                Logger.LogError(e, $"OpenDownloadDialog {nameof(HttpRequestException)}");
            }
            catch (Exception e)
            {
                await PopupServiceHelper.Error(I18n.T("Backups.Download.Fail"));
                Logger.LogError(e, $"Backups Download Fail");
            }

        }

        private async Task Download(string fileName)
        {
            showDownload = false;

            await PopupServiceHelper.StartLoading();
            var destFileName = webDavFolderName + "/" + fileName;
            try
            {
                using var stream = await WebDAVService.DownloadAsync(destFileName);
                await DiaryFileManager.ImportDBAsync(stream);
                await PopupServiceHelper.Success(I18n.T("Backups.Download.Success"));
            }
            catch (HttpRequestException e)
            {
                await PopupServiceHelper.Error(I18n.T("Backups.Config.Fail.Network"));
                Logger.LogError(e, $"OpenDownloadDialog {nameof(HttpRequestException)}");
            }
            catch (Exception e)
            {
                await PopupServiceHelper.Error(I18n.T("Backups.Download.Fail"));
                Logger.LogError(e, "WebDAV Download fail");
            }
            finally
            {
                await PopupServiceHelper.StopLoading();
            }
        }

        private async Task<bool> Check()
        {
            if (!Configured)
            {
                await PopupServiceHelper.Error(I18n.T("Backups.Config.CheckConfigured"));
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
