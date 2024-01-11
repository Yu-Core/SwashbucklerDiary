using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.WebAssembly.Essentials;

namespace SwashbucklerDiary.WebAssembly.Services
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

        private readonly IVersionTracking _versionTracking;

        public VersionUpdataManager(IDiaryService diaryService,
            IResourceService resourceService,
            Rcl.Essentials.IPreferences preferences,
            IMediaResourceManager mediaResourceManager,
            IVersionTracking versionTracking)
        {
            _diaryService = diaryService;
            _resourceService = resourceService;
            _preferences = preferences;
            _mediaResourceManager = mediaResourceManager;
            _versionTracking = versionTracking;
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
            //await UpdateVersion("0.64.7", UpdateVersion647);

            if (AfterUpdateVersion != null && updateCount > 0)
            {
                await AfterUpdateVersion.Invoke();
            }
        }

        private async Task UpdateVersion(string version, Func<Task> func)
        {
            string? strPreviousVersion = _versionTracking.PreviousVersion;
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

            bool first = _versionTracking.IsFirstLaunchForCurrentVersion;
            if (!first)
            {
                return;
            }

            updateCount++;
            await func.Invoke();
        }
    }
}
