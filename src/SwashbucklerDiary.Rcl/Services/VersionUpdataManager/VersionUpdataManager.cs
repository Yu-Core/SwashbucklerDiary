namespace SwashbucklerDiary.Rcl.Services
{
    public abstract class VersionUpdataManager : IVersionUpdataManager
    {
        public event Func<Task>? AfterFirstEnter;

        public event Func<Task>? AfterUpdateVersion;

        protected int updateCount;

        protected readonly IDiaryService _diaryService;

        protected readonly IResourceService _resourceService;

        protected readonly Rcl.Essentials.IPreferences _preferences;

        protected readonly IMediaResourceManager _mediaResourceManager;

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

        public virtual async Task UpdateVersion()
        {
            if (AfterUpdateVersion != null && updateCount > 0)
            {
                await AfterUpdateVersion.Invoke();
            }
        }

        protected abstract string? PreviousVersion { get; }

        protected abstract bool IsFirstLaunchForCurrentVersion { get; }

        protected async Task UpdateVersion(string version, Func<Task> func)
        {
            string? strPreviousVersion = PreviousVersion;
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

            bool first = IsFirstLaunchForCurrentVersion;
            if (!first)
            {
                return;
            }

            updateCount++;
            await func.Invoke();
        }

        protected abstract Task UpdateVersion697();

    }
}
