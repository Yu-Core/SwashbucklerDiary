using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class MediaResourceManager : Rcl.Services.MediaResourceManager
    {
        public MediaResourceManager(IPlatformIntegration platformIntegration,
            IAppFileSystem appFileSystem,
            II18nService i18nService,
            ISettingService settingService,
            ILogger<MediaResourceManager> logger) :
            base(platformIntegration, appFileSystem, i18nService, settingService, logger)
        {
        }

        public override string RelativeUrlToFilePath(string urlRelativePath) => urlRelativePath;

        public override string FilePathToRelativeUrl(string filePath) => filePath;

        public override async Task<string?> ToFilePathAsync(MediaResourcePath? path)
        {
            if (path is null)
            {
                return null;
            }

            string? filePath = null;
            if (path.RelativePathOfBaseUri is string relativePath)
            {
                filePath = RelativeUrlToFilePath(relativePath);
            }

            return await Task.FromResult(filePath).ConfigureAwait(false);
        }
    }
}
