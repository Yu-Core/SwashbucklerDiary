using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Gtk.Services
{
    public class MediaResourceManager : Rcl.Hybird.Services.MediaResourceManager
    {
        public MediaResourceManager(IPlatformIntegration platformIntegration,
            IAppFileSystem appFileSystem,
            II18nService i18nService,
            ILogger<MediaResourceManager> logger)
            : base(platformIntegration, appFileSystem, i18nService, logger)
        {
        }
    }
}
