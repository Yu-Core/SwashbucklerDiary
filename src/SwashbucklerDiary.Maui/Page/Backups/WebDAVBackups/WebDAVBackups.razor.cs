using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Pages
{
    // TODO: 写的不好，暂时先将就着，以后需要重写
    public partial class WebDAVBackups : ImportantComponentBase
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
        private ILogger<WebDAVBackups> Logger { get; set; } = default!;

        [Inject]
        private IDiaryFileManager DiaryFileManager { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if(firstRender)
            {
                await LoadSettings();
                StateHasChanged();
            }
        }

        private bool Configured => !string.IsNullOrEmpty(configModel.ServerAddress);

        private string ConfiguredText => Configured ? I18n.T("Backups.Config.Configured") : I18n.T("Backups.Config.NotConfigured");


        private async Task LoadSettings()
        {
            configModel.ServerAddress = await Preferences.Get<string>(Setting.WebDAVServerAddress);
            configModel.Account = await Preferences.Get<string>(Setting.WebDAVAccount);
            configModel.Password = await Preferences.Get<string>(Setting.WebDAVPassword);
            includeDiaryResources = await Preferences.Get<bool>(Setting.WebDAVCopyResources);
        }

        private async Task SaveWebDavConfig(WebDavConfigForm webDavConfig)
        {
            bool isSuccess = await SetWebDav(webDavConfig);
            if (isSuccess)
            {
                configModel = webDavConfig.DeepCopy();
                showConfig = false;
                await AlertService.Success(I18n.T("Backups.Config.Success"));
                await Preferences.Set(Setting.WebDAVServerAddress, configModel.ServerAddress);
                await Preferences.Set(Setting.WebDAVAccount, configModel.Account);
                await Preferences.Set(Setting.WebDAVPassword, configModel.Password);
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
                await AlertService.Error(I18n.T("Backups.Config.Fail.ConfigInfo"));
                Logger.LogError(e, $"SaveWebDavConfig {nameof(ArgumentException)}");
            }
            catch (HttpRequestException e)
            {
                await AlertService.Error(I18n.T("Backups.Config.Fail.Network"));
                Logger.LogError(e, $"SaveWebDavConfig {nameof(HttpRequestException)}");
            }
            catch (WebDAVException e)
            {
                await AlertService.Error(I18n.T("Backups.Config.Fail.ConfigInfo"));
                Logger.LogError(e, $"SaveWebDavConfig {nameof(WebDAVException)}");
            }
            catch (Exception e)
            {
                await AlertService.Error(I18n.T("Backups.Config.Fail.Unknown"));
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

            await AlertService.StartLoading();

            string filePath = await DiaryFileManager.ExportDBAsync(includeDiaryResources);
            using var stream = File.OpenRead(filePath);
            string fileName = await DiaryFileManager.GetBackupFileName();
            string destFileName = webDavFolderName + "/" + fileName;
            try
            {
                await WebDAVService.UploadAsync(destFileName, stream);
                await AlertService.Success(I18n.T("Backups.Upload.Success"));
            }
            catch (HttpRequestException e)
            {
                await AlertService.Error(I18n.T("Backups.Config.Fail.Network"));
                Logger.LogError(e, $"OpenDownloadDialog {nameof(HttpRequestException)}");
            }
            catch (Exception e)
            {
                await AlertService.Error(I18n.T("Backups.Upload.Fail"));
                Logger.LogError(e, $"Backups Upload Fail");
            }
            finally
            {
                await AlertService.StopLoading();
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
                await AlertService.Error(I18n.T("Backups.Config.Fail.Network"));
                Logger.LogError(e, $"OpenDownloadDialog {nameof(HttpRequestException)}");
            }
            catch (Exception e)
            {
                await AlertService.Error(I18n.T("Backups.Download.Fail"));
                Logger.LogError(e, $"Backups Download Fail");
            }

        }

        private async Task Download(string fileName)
        {
            showDownload = false;

            await AlertService.StartLoading();
            var destFileName = webDavFolderName + "/" + fileName;
            try
            {
                using var stream = await WebDAVService.DownloadAsync(destFileName);
                await DiaryFileManager.ImportDBAsync(stream);
                await AlertService.Success(I18n.T("Backups.Download.Success"));
            }
            catch (HttpRequestException e)
            {
                await AlertService.Error(I18n.T("Backups.Config.Fail.Network"));
                Logger.LogError(e, $"OpenDownloadDialog {nameof(HttpRequestException)}");
            }
            catch (Exception e)
            {
                await AlertService.Error(I18n.T("Backups.Download.Fail"));
                Logger.LogError(e, "WebDAV Download fail");
            }
            finally
            {
                await AlertService.StopLoading();
            }
        }

        private async Task<bool> Check()
        {
            if (!Configured)
            {
                await AlertService.Error(I18n.T("Backups.Config.CheckConfigured"));
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
