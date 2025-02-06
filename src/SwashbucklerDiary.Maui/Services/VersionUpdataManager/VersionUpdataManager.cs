using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Pages;
using SwashbucklerDiary.Rcl.Services;
using System.Text.Json;

namespace SwashbucklerDiary.Maui.Services
{
    public class VersionUpdataManager : Rcl.Services.VersionUpdataManager
    {
        private readonly IAccessExternal _accessExternal;

        private readonly IPopupServiceHelper _popupServiceHelper;

        public VersionUpdataManager(IDiaryService diaryService,
            IResourceService resourceService,
            ISettingService settingService,
            IMediaResourceManager mediaResourceManager,
            II18nService i18n,
            Rcl.Essentials.IVersionTracking versionTracking,
            IDiaryFileManager diaryFileManager,
            IAccessExternal accessExternal,
            IPopupServiceHelper popupServiceHelper,
            IStaticWebAssets staticWebAssets) :
            base(diaryService, resourceService, settingService, mediaResourceManager, i18n, versionTracking, diaryFileManager, staticWebAssets)
        {
            _accessExternal = accessExternal;
            _popupServiceHelper = popupServiceHelper;
        }

        public override async Task HandleVersionUpdate()
        {
            await HandleVersionUpdate("0.64.7", HandleVersionUpdate647);
            await base.HandleVersionUpdate();
        }

        //此版本之后更改了资源的链接
        private async Task HandleVersionUpdate647()
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
            bool flag = await _accessExternal.OpenAppStoreAppDetails();
            if (!flag)
            {
                await _popupServiceHelper.Error(_i18n.T("About.OpenAppStoreFail"));
            }
        }
    }
}
