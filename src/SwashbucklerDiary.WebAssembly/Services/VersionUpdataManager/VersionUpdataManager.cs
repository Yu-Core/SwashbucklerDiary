using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class VersionUpdataManager : Rcl.Services.VersionUpdataManager
    {
        public VersionUpdataManager(IDiaryService diaryService,
            IResourceService resourceService,
            ISettingService settingService,
            IMediaResourceManager mediaResourceManager,
            II18nService i18n,
            IVersionTracking versionTracking,
            IDiaryFileManager diaryFileManager) :
            base(diaryService, resourceService, settingService, mediaResourceManager, i18n, versionTracking, diaryFileManager)
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
            await _settingService.Remove(keys);
        }
    }
}
