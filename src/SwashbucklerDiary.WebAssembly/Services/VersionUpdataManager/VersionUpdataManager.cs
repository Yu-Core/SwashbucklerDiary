using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.WebAssembly.Essentials;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class VersionUpdataManager :  Rcl.Services.VersionUpdataManager
    {
        private readonly IVersionTracking _versionTracking;

        public VersionUpdataManager(IDiaryService diaryService, 
            IResourceService resourceService, 
            IPreferences preferences, 
            IMediaResourceManager mediaResourceManager,
            IVersionTracking versionTracking) : 
            base(diaryService, resourceService, preferences, mediaResourceManager)
        {
            _versionTracking = versionTracking;
        }

        protected override string? PreviousVersion => _versionTracking.PreviousVersion;

        protected override bool IsFirstLaunchForCurrentVersion => _versionTracking.IsFirstLaunchForCurrentVersion;

        public override async Task UpdateVersion()
        {
            await UpdateVersion("0.69.7", UpdateVersion697);
            await base.UpdateVersion();
        }

        protected override async Task UpdateVersion697()
        {
            string[] keys = ["ThemeState", "Date"];
            await _preferences.Remove(keys);
        }
    }
}
