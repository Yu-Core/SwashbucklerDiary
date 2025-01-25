using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Gtk.Services
{
    public class VersionUpdataManager : Rcl.Services.VersionUpdataManager
    {
        private readonly IPlatformIntegration _platformIntegration;

        public VersionUpdataManager(IDiaryService diaryService,
            IResourceService resourceService,
            ISettingService settingService,
            IMediaResourceManager mediaResourceManager,
            II18nService i18n,
            Rcl.Essentials.IVersionTracking versionTracking,
            IDiaryFileManager diaryFileManager,
            IPlatformIntegration platformIntegration,
            IStaticWebAssets staticWebAssets) :
            base(diaryService, resourceService, settingService, mediaResourceManager, i18n, versionTracking, diaryFileManager, staticWebAssets)
        {
            _platformIntegration = platformIntegration;
        }

        public override async Task ToUpdate()
        {
            await _platformIntegration.OpenBrowser("https://github.com/Yu-Core/SwashbucklerDiary/releases");
        }
    }
}
