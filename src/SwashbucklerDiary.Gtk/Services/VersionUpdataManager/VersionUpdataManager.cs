using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Gtk.Services
{
    public class VersionUpdataManager : Rcl.Services.VersionUpdataManager
    {
        public VersionUpdataManager(IDiaryService diaryService,
            IResourceService resourceService,
            ISettingService settingService,
            IMediaResourceManager mediaResourceManager,
            II18nService i18n,
            Rcl.Essentials.IVersionTracking versionTracking,
            IDiaryFileManager diaryFileManager,
            IStaticWebAssets staticWebAssets,
            IAppFileSystem appFileSystem,
            IAvatarService avatarService) :
            base(diaryService, resourceService, settingService, mediaResourceManager, i18n, versionTracking, diaryFileManager, staticWebAssets, appFileSystem, avatarService)
        {
        }
    }
}
