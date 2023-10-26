using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Services
{
    public class VersionService : IVersionService
    {
        private int UpdateCount;

        private readonly IDiaryService DiaryService;

        private readonly IResourceService ResourceService;

        private readonly ISettingsService SettingsService;

        public event Func<Task>? AfterFirstLauch;

        public event Func<Task>? AfterUpdateVersion;

        public VersionService(IDiaryService diaryService, 
            IResourceService resourceService,
            ISettingsService settingsService)
        {
            DiaryService = diaryService;
            ResourceService = resourceService;
            SettingsService = settingsService;
        }

        public async Task NotifyFirstLauchChanged()
        {
            if (AfterFirstLauch == null)
            {
                return;
            }

            await AfterFirstLauch.Invoke();
        }

        public async Task UpdateVersion()
        {
            await UpdateVersion("0.64.7", UpdateVersion647);

            if (AfterUpdateVersion != null && UpdateCount > 0)
            {
                await AfterUpdateVersion.Invoke();
            }
        }

        private async Task UpdateVersion(string version, Func<Task> func)
        {
            string? strPreviousVersion = VersionTracking.Default.PreviousVersion;
            if (string.IsNullOrEmpty(strPreviousVersion))
            {
                return;
            }

            var previousVersion = new Version(strPreviousVersion);
            var targetVersion = new Version(version);
            int result = previousVersion.CompareTo(targetVersion);
            if (result > 0)
            {
                return;
            }

            bool first = VersionTracking.Default.IsFirstLaunchForCurrentVersion;
            if (!first)
            {
                return;
            }

            UpdateCount++;
            await func.Invoke();
        }

        //此版本之后更改了资源的链接
        private async Task UpdateVersion647()
        {
            string avatar = await SettingsService.Get(SettingType.Avatar);
            avatar = avatar.Replace("appdata:///", "appdata/");
            await SettingsService.Save(SettingType.Avatar, avatar);

            var diaries = await DiaryService.QueryAsync();
            await ResourceService.DeleteAsync();
            foreach (var diary in diaries)
            {
                diary.Content = diary.Content!.Replace("appdata:///", "appdata/");
                diary.Resources = ResourceService.GetDiaryResources(diary.Content);
                diary.UpdateTime = DateTime.Now;
            }
            await DiaryService.UpdateIncludesAsync(diaries);
        }
    }
}
