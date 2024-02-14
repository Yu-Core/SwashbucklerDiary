using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Text.Json;

namespace SwashbucklerDiary.Maui.Services
{
    public class VersionUpdataManager : Rcl.Services.VersionUpdataManager
    {
        public VersionUpdataManager(IDiaryService diaryService,
            IResourceService resourceService,
            ISettingService settingService,
            IMediaResourceManager mediaResourceManager,
            II18nService i18n,
            Rcl.Essentials.IVersionTracking versionTracking) :
            base(diaryService, resourceService, settingService, mediaResourceManager, i18n, versionTracking)
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
            string avatar = await _settingService.Get<string>(Setting.Avatar);
            avatar = avatar.Replace("appdata:///", "appdata/");
            await _settingService.Set(Setting.Avatar, avatar);

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
    }
}
