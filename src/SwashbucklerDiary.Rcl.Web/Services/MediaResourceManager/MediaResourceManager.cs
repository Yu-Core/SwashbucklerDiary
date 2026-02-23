using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Web.Services
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

        public override Task<string?> ToFilePathAsync(MediaResourcePath? path)
        {
            if (path is null)
            {
                return Task.FromResult<string?>(null);
            }

            string? filePath = null;
            if (path.RelativePathOfBaseUri is string relativePath)
            {
                filePath = RelativeUrlToFilePath(relativePath);
            }

            return Task.FromResult<string?>(filePath);
        }
    }
}
