using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class VersionUpdataManager : Rcl.Services.VersionUpdataManager
    {
        private readonly IPlatformIntegration _platformIntegration;

        public VersionUpdataManager(IDiaryService diaryService,
            IResourceService resourceService,
            ISettingService settingService,
            IMediaResourceManager mediaResourceManager,
            II18nService i18n,
            IVersionTracking versionTracking,
            IDiaryFileManager diaryFileManager,
            IPlatformIntegration platformIntegration,
            IStaticWebAssets staticWebAssets,
            IAppFileSystem appFileSystem,
            IAvatarService avatarService) :
            base(diaryService, resourceService, settingService, mediaResourceManager, i18n, versionTracking, diaryFileManager, staticWebAssets, appFileSystem, avatarService)
        {
            _platformIntegration = platformIntegration;
        }

        public override async Task ToUpdate()
        {
            await _platformIntegration.OpenBrowser("https://github.com/Yu-Core/SwashbucklerDiary/tree/gh-pages").ConfigureAwait(false);
        }

        protected override async Task HandleVersionUpdate697()
        {
            string[] keys = ["ThemeState", "Date"];
            await _settingService.RemoveAsync(keys).ConfigureAwait(false);
        }
    }
}
