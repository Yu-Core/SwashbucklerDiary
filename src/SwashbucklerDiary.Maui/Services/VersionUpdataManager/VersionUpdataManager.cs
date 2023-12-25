using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Services
{
    public class VersionUpdataManager : IVersionUpdataManager
    {
        public event Func<Task>? AfterFirstEnter;

        public event Func<Task>? AfterUpdateVersion;

        private int updateCount;

        private readonly IDiaryService _diaryService;

        private readonly IResourceService _resourceService;

        private readonly Rcl.Essentials.IPreferences _preferences;

        private readonly IMediaResourceManager _mediaResourceManager;

        public VersionUpdataManager(IDiaryService diaryService,
            IResourceService resourceService,
            Rcl.Essentials.IPreferences preferences,
            IMediaResourceManager mediaResourceManager)
        {
            _diaryService = diaryService;
            _resourceService = resourceService;
            _preferences = preferences;
            _mediaResourceManager = mediaResourceManager;
        }

        public async Task FirstEnter()
        {
            if (AfterFirstEnter == null)
            {
                return;
            }

            await AfterFirstEnter.Invoke();
        }

        public async Task UpdateVersion()
        {
            await UpdateVersion("0.64.7", UpdateVersion647);

            if (AfterUpdateVersion != null && updateCount > 0)
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

            updateCount++;
            await func.Invoke();
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
    }
}
