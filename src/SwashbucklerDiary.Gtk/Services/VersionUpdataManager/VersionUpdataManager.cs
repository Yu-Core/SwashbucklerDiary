using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Gtk.Services
{
    public class VersionUpdataManager : Rcl.Services.VersionUpdataManager
    {
        private readonly IPlatformIntegration _platformIntegration;
        private readonly IAppFileSystem _appFileSystem;

        public VersionUpdataManager(IDiaryService diaryService,
            IResourceService resourceService,
            ISettingService settingService,
            IMediaResourceManager mediaResourceManager,
            II18nService i18n,
            Rcl.Essentials.IVersionTracking versionTracking,
            IDiaryFileManager diaryFileManager,
            IPlatformIntegration platformIntegration,
            IStaticWebAssets staticWebAssets,
            IAppFileSystem appFileSystem) :
            base(diaryService, resourceService, settingService, mediaResourceManager, i18n, versionTracking, diaryFileManager, staticWebAssets)
        {
            _platformIntegration = platformIntegration;
            _appFileSystem = appFileSystem;
        }

        public override async Task ToUpdate()
        {
            await _platformIntegration.OpenBrowser("https://github.com/Yu-Core/SwashbucklerDiary/releases");
        }

        protected override void InitializeVersionHandlers()
        {
            base.InitializeVersionHandlers();

            AddVersionHandler("1.11.7", HandleVersionUpdate1117);
        }

        private Task HandleVersionUpdate1117()
        {
            string _oldAppDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppInfo.PackageName, "Data");
            _appFileSystem.MoveFolder(_oldAppDataDirectory, FileSystem.AppDataDirectory, SearchOption.AllDirectories);
            return Task.CompletedTask;
        }
    }
}
