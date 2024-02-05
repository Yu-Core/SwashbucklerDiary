using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class VersionUpdataManager :  Rcl.Services.VersionUpdataManager
    {
        public VersionUpdataManager(IDiaryService diaryService, 
            IResourceService resourceService, 
            IPreferences preferences, 
            IMediaResourceManager mediaResourceManager,
            II18nService i18n,
            IVersionTracking versionTracking) : 
            base(diaryService, resourceService, preferences, mediaResourceManager, i18n, versionTracking)
        {
        }

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
