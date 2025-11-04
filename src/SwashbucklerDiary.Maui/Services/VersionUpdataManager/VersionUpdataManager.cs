using Microsoft.Data.SqlClient;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Pages;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Text.Json;

namespace SwashbucklerDiary.Maui.Services
{
    public class VersionUpdataManager : Rcl.Services.VersionUpdataManager
    {
        private readonly IAccessExternal _accessExternal;

        private readonly IAlertService _alertService;

        private readonly IPlatformIntegration _platformIntegration;

        public VersionUpdataManager(IDiaryService diaryService,
            IResourceService resourceService,
            ISettingService settingService,
            IMediaResourceManager mediaResourceManager,
            II18nService i18n,
            Rcl.Essentials.IVersionTracking versionTracking,
            IDiaryFileManager diaryFileManager,
            IAccessExternal accessExternal,
            IAlertService alertService,
            IStaticWebAssets staticWebAssets,
            IPlatformIntegration platformIntegration,
            IAppFileSystem appFileSystem,
            IAvatarService avatarService) :
            base(diaryService, resourceService, settingService, mediaResourceManager, i18n, versionTracking, diaryFileManager, staticWebAssets, appFileSystem, avatarService)
        {
            _accessExternal = accessExternal;
            _alertService = alertService;
            _platformIntegration = platformIntegration;
        }

        protected override void InitializeVersionHandlers()
        {
            base.InitializeVersionHandlers();

            AddVersionHandler("0.65.5", HandleVersionUpdate655);
        }

        //此版本之后更改了资源的链接
        private async Task HandleVersionUpdate655()
        {
            var key = "Avatar";
            string avatar = await _settingService.GetAsync<string>(key, string.Empty);
            if (avatar != string.Empty)
            {
                avatar = avatar.Replace("appdata:///", "appdata/");
                await _settingService.SetAsync(key, avatar);
            }

            await _diaryFileManager.AllUseNewResourceUriAsync();
        }

        protected override async Task HandleVersionUpdate697()
        {
            var webDAVServerAddressTask = _settingService.GetAsync<string>("WebDAVServerAddress", string.Empty);
            var webDAVAccountTask = _settingService.GetAsync<string>("WebDAVAccount", string.Empty);
            var webDAVPasswordTask = _settingService.GetAsync<string>("WebDAVPassword", string.Empty);
            await Task.WhenAll(webDAVServerAddressTask, webDAVAccountTask, webDAVPasswordTask);
            var webDAVServerAddress = webDAVServerAddressTask.Result;
            if (!string.IsNullOrEmpty(webDAVServerAddress))
            {
                var config = new WebDavConfigForm()
                {
                    ServerAddress = webDAVServerAddressTask.Result,
                    Account = webDAVAccountTask.Result,
                    Password = webDAVPasswordTask.Result,
                };
                string webDavConfigJson = JsonSerializer.Serialize(config);
                await _settingService.SetAsync("WebDavConfig", webDavConfigJson);
            }

            string[] keys = ["ThemeState", "WebDAVServerAddress", "WebDAVAccount", "WebDAVPassword", "Date"];
            await _settingService.RemoveAsync(keys);
        }

        public override async Task ToUpdate()
        {
#if WINDOWS
            bool IsPackagedApp = false;
            try
            {
                if (Windows.ApplicationModel.Package.Current is not null)
                {
                    IsPackagedApp = true;
                }
            }
            catch
            {
            }
            
            if (!IsPackagedApp)
            {
                await _platformIntegration.OpenBrowser("https://github.com/Yu-Core/SwashbucklerDiary/releases");
                return;
            }
#endif
            bool flag = await _accessExternal.OpenAppStoreAppDetails();
            if (!flag)
            {
                await _alertService.ErrorAsync(_i18n.T("Failed to open the application store"));
            }
        }

#if IOS || MACCATALYST
        protected override async Task HandleVersionUpdate1291()
        {
            Dictionary<string, string> directoryPaths = new()
            {
                ["Image"] = _mediaResourceManager.MediaResourceDirectoryPaths[MediaResource.Image],
                ["Video"] = _mediaResourceManager.MediaResourceDirectoryPaths[MediaResource.Video],
                ["Avatar"] = _avatarService.AvatarDirectoryPath,
            };
            foreach (var item in directoryPaths)
            {
                var oldPath = Path.Combine(FileSystem.AppDataDirectory, item.Key.ToString());
                if (Directory.Exists(oldPath))
                {
                    await _appFileSystem.MoveFolderAsync(oldPath, item.Value).ConfigureAwait(false);
                }
            }

            SqlConnection.ClearAllPools();
            await Task.Run(() =>
            {
                var oldDatabasePath = Path.Combine(FileSystem.AppDataDirectory, SQLiteConstants.DatabaseFilename);
                _appFileSystem.FileMove(oldDatabasePath, SQLiteConstants.DatabasePath, true);
                var oldPrivacyDatabasePath = Path.Combine(FileSystem.AppDataDirectory, SQLiteConstants.PrivacyDatabaseFilename);
                _appFileSystem.FileMove(oldPrivacyDatabasePath, SQLiteConstants.PrivacyDatabasePath, true);
            });

            await base.HandleVersionUpdate1291();
        }
#endif
    }
}
