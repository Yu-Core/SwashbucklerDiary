using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class DiaryFileManager : Rcl.Services.DiaryFileManager
    {
        public DiaryFileManager(IAppFileSystem appFileSystem,
            IPlatformIntegration platformIntegration,
            II18nService i18nService,
            IMediaResourceManager mediaResourceManager,
            IDiaryService diaryService,
            IResourceService resourceService,
            ISettingService settingService)
            : base(appFileSystem, platformIntegration, i18nService, mediaResourceManager, diaryService, resourceService, settingService)
        {
        }

        protected override string GetDatabasePath()
        {
            return SQLiteConstants.DatabasePath;
        }
    }
}
