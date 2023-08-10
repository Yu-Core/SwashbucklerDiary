using Serilog;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Extend;
using SwashbucklerDiary.Models;
using System.Net;
using WebDav;

namespace SwashbucklerDiary.Pages
{
    public partial class WebDAVBackups : BackupsPageComponentBase
    {
        private bool ShowConfig;
        private bool ShowUpload;
        private bool ShowDownload;
        private bool IncludeDiaryResources;
        private WebDavConfigForm configModel = new();
        private const string WebDavFolderName = "SwashbucklerDiary";
        private IWebDavClient WebDavClient = default!;
        public List<string> FileList = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await base.OnInitializedAsync();
        }

        private bool Configured => !string.IsNullOrEmpty(configModel.ServerAddress);
        private string ConfiguredText => Configured ? I18n.T("Backups.Config.Configured") : I18n.T("Backups.Config.NotConfigured");

        private async Task LoadSettings()
        {
            configModel.ServerAddress = await SettingsService.Get(SettingType.WebDAVServerAddress);
            configModel.Account = await SettingsService.Get(SettingType.WebDAVAccount);
            configModel.Password = await SettingsService.Get(SettingType.WebDAVPassword);
            IncludeDiaryResources = await SettingsService.Get(SettingType.WebDAVCopyResources);
        }

        private async Task<bool> ConnectWebDavClient(WebDavConfigForm webDavConfig)
        {
            bool uriResult = Uri.TryCreate(webDavConfig.ServerAddress, UriKind.Absolute, out Uri? uri);
            if (!uriResult)
            {
                await AlertService.Error(I18n.T("Backups.Config.Fail.ConfigInfo"));
                return false;
            }

            string? username = webDavConfig.Account;
            string? password = webDavConfig.Password;
            var httpHandler = new SocketsHttpHandler()
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                Credentials = new NetworkCredential(username, password)
            };
            var client = new HttpClient(httpHandler, true) { BaseAddress = uri };
            var webDavClient = new WebDavClient(client);

            try
            {
                var result = await webDavClient.Propfind(WebDavFolderName);
                if (!result.IsSuccessful)
                {
                    if (result.StatusCode == (int)HttpStatusCode.Unauthorized)
                    {
                        await AlertService.Error(I18n.T("Backups.Config.Fail.ConfigInfo"));
                        return false;
                    }

                    var result2 = await webDavClient.Mkcol(WebDavFolderName);
                    if (!result2.IsSuccessful)
                    {
                        await AlertService.Error(I18n.T("Backups.Config.Fail.Unknown"));
                        return false;
                    }
                }

                if (WebDavClient != webDavClient)
                {
                    WebDavClient = webDavClient;
                }

                configModel = webDavConfig.DeepCopy();
            }
            catch (HttpRequestException e)
            {
                Log.Error($"{e.Message}\n{e.StackTrace}");
                await AlertService.Error(I18n.T("Backups.Config.Fail.NoNetwork"));
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}\n{e.StackTrace}");
                await AlertService.Error(I18n.T("Backups.Config.Fail.Unknown"));
                return false;
            }

            return true;
        }

        private async Task SaveWebDavConfig(WebDavConfigForm webDavConfig)
        {
            bool flag = await ConnectWebDavClient(webDavConfig);
            if (flag)
            {
                ShowConfig = false;
                await AlertService.Success(I18n.T("Backups.Config.Success"));
                await SettingsService.Save(SettingType.WebDAVServerAddress, configModel.ServerAddress);
                await SettingsService.Save(SettingType.WebDAVAccount, configModel.Account);
                await SettingsService.Save(SettingType.WebDAVPassword, configModel.Password);
            }
        }

        private async Task OpenUploadDialog()
        {
            if (!Configured)
            {
                await AlertService.Error(I18n.T("Backups.Config.CheckConfigured"));
                return;
            }

            var flag = await CheckPermissions();
            if (!flag)
            {
                return;
            }

            ShowUpload = true;
        }

        private async Task Upload()
        {
            ShowUpload = false;

            bool flag2 = await ConnectWebDavClient(configModel);
            if (!flag2)
            {
                return;
            }

            await AlertService.StartLoading();
            var diaries = await DiaryService.QueryAsync();
            bool copyResources = await SettingsService.Get(SettingType.WebDAVCopyResources);
            using var stream = await AppDataService.BackupDatabase(diaries, copyResources);
            var destFileName = WebDavFolderName + "/" + AppDataService.GetBackupFileName();

            await WebDavClient.PutFile(destFileName, stream);
            await AlertService.StopLoading();
            await AlertService.Success(I18n.T("Backups.Upload.Success"));
        }

        private async Task OpenDownloadDialog()
        {
            if (!Configured)
            {
                await AlertService.Error(I18n.T("Backups.Config.CheckConfigured"));
                return;
            }

            var flag = await CheckPermissions();
            if (!flag)
            {
                return;
            }

            bool flag2 = await ConnectWebDavClient(configModel);
            if (!flag2)
            {
                return;
            }
            await SetFileList();
            ShowDownload = true;
        }

        private async Task Download(string fileName)
        {
            ShowDownload = false;

            bool flag2 = await ConnectWebDavClient(configModel);
            if (!flag2)
            {
                return;
            }

            await AlertService.StartLoading();
            var destFileName = WebDavFolderName + "/" + fileName;
            using var response = await WebDavClient.GetRawFile(destFileName); // get a file without processing from the server
            if (!response.IsSuccessful)
            {
                await AlertService.Error(I18n.T("Backups.Download.Fail"));
                return;
            }

            var flag = await AppDataService.RestoreDatabase(response.Stream);
            await AlertService.StopLoading();
            if (flag)
            {
                await AlertService.Success(I18n.T("Backups.Download.Success"));
            }
            else
            {
                await AlertService.Error(I18n.T("Backups.Download.Fail"));
            }
        }

        private async Task SetFileList()
        {
            List<string> fileList = new();
            var result = await WebDavClient.Propfind(WebDavFolderName);
            if (result.IsSuccessful)
            {
                foreach (var res in result.Resources)
                {
                    if (res.IsCollection)
                    {
                        continue;
                    }

                    string extension = Path.GetExtension(res.DisplayName);
                    if (extension == ".zip")
                    {
                        fileList.Add(res.DisplayName);
                    }
                }
            }
            FileList = fileList;
            FileList.Reverse();
        }

        private Func<bool, Task> SettingChange(SettingType type)
        {
            return (bool value) => SettingsService.Save(type, value);
        }

        private string? MSwitchTrackColor(bool value)
        {
            return value && Light ? "black" : null;
        }
    }
}
