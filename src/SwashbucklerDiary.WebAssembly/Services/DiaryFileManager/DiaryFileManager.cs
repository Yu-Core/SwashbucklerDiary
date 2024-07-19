using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class DiaryFileManager : Rcl.Services.DiaryFileManager
    {
        public DiaryFileManager(IAppFileManager appFileManager,
            IPlatformIntegration platformIntegration,
            II18nService i18nService,
            IMediaResourceManager mediaResourceManager,
            IDiaryService diaryService,
            IResourceService resourceService)
            : base(appFileManager, platformIntegration, i18nService, mediaResourceManager, diaryService, resourceService)
        {
        }

        protected override string GetDatabasePath()
        {
            return SQLiteConstants.DatabasePath;
        }
    }
}
