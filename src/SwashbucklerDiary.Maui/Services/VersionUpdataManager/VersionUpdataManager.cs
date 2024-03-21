using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Text.Json;

namespace SwashbucklerDiary.Maui.Services
{
    public class VersionUpdataManager : Rcl.Services.VersionUpdataManager
    {
        private readonly IAccessExternal _accessExternal;

        private readonly IAlertService _alertService;

        public VersionUpdataManager(IDiaryService diaryService,
            IResourceService resourceService,
            ISettingService settingService,
            IMediaResourceManager mediaResourceManager,
            II18nService i18n,
            Rcl.Essentials.IVersionTracking versionTracking,
            IDiaryFileManager diaryFileManager,
            IAccessExternal accessExternal,
            IAlertService alertService) :
            base(diaryService, resourceService, settingService, mediaResourceManager, i18n, versionTracking, diaryFileManager)
        {
            _accessExternal = accessExternal;
            _alertService = alertService;
        }

        public override async Task HandleVersionUpdate()
        {
            await HandleVersionUpdate("0.64.7", HandleVersionUpdate647);
            await HandleVersionUpdate("0.69.7", HandleVersionUpdate697);
            await base.HandleVersionUpdate();
        }

        //此版本之后更改了资源的链接
        private async Task HandleVersionUpdate647()
        {
            string avatar = await _settingService.Get<string>("Avatar", "");
            avatar = avatar.Replace("appdata:///", "appdata/");
            await _settingService.Set(Setting.Avatar, avatar);

            await _diaryFileManager.UpdateAllResourceUri();
        }

        protected override async Task HandleVersionUpdate697()
        {
            var webDAVServerAddressTask = _settingService.Get<string>("WebDAVServerAddress", string.Empty);
            var webDAVAccountTask = _settingService.Get<string>("WebDAVAccount", string.Empty);
            var webDAVPasswordTask = _settingService.Get<string>("WebDAVPassword", string.Empty);
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
                await _settingService.Set(Setting.WebDavConfig, webDavConfigJson);
            }

            string[] keys = ["ThemeState", "WebDAVServerAddress", "WebDAVAccount", "WebDAVPassword", "Date"];
            await _settingService.Remove(keys);
        }

        public override async Task ToUpdate()
        {
            bool flag = await _accessExternal.OpenAppStoreAppDetails();
            if (!flag)
            {
                await _alertService.Error(_i18n.T("About.OpenAppStoreFail"));
            }
        }
    }
}
