using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Text.Json;

namespace SwashbucklerDiary.Maui.Services
{
    public class VersionUpdataManager : Rcl.Services.VersionUpdataManager
    {
        public VersionUpdataManager(IDiaryService diaryService,
            IResourceService resourceService,
            Rcl.Essentials.IPreferences preferences,
            IMediaResourceManager mediaResourceManager,
            II18nService i18n,
            Rcl.Essentials.IVersionTracking versionTracking) :
            base(diaryService, resourceService, preferences, mediaResourceManager, i18n, versionTracking)
        {
        }

        public override async Task UpdateVersion()
        {
            await UpdateVersion("0.64.7", UpdateVersion647);
            await UpdateVersion("0.69.7", UpdateVersion697);
            await base.UpdateVersion();
        }

        //此版本之后更改了资源的链接
        private async Task UpdateVersion647()
        {
            string avatar = await _preferences.Get<string>(Setting.Avatar);
            avatar = avatar.Replace("appdata:///", "appdata/");
            await _preferences.Set(Setting.Avatar, avatar);

            var diaries = await _diaryService.QueryAsync();
            await _resourceService.DeleteAsync();
            foreach (var diary in diaries)
            {
                diary.Content = diary.Content!.Replace("appdata:///", "appdata/");
                diary.Resources = _mediaResourceManager.GetDiaryResources(diary.Content);
                diary.UpdateTime = DateTime.Now;
            }
            await _diaryService.UpdateIncludesAsync(diaries);
        }

        protected override async Task UpdateVersion697()
        {
            var webDAVServerAddressTask = _preferences.Get<string>("WebDAVServerAddress");
            var webDAVAccountTask = _preferences.Get<string>("WebDAVAccount");
            var webDAVPasswordTask = _preferences.Get<string>("WebDAVPassword");
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
                await _preferences.Set(Setting.WebDavConfig, webDavConfigJson);
            }

            string[] keys = ["ThemeState", "WebDAVServerAddress", "WebDAVAccount", "WebDAVPassword", "Date"];
            await _preferences.Remove(keys);
        }
    }
}
