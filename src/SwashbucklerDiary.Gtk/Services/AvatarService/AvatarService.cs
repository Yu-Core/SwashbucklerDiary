using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Gtk.Services
{
    public class AvatarService : Rcl.Services.AvatarService
    {
        public AvatarService(ISettingService settingService,
            IMediaResourceManager mediaResourceManager,
            IPlatformIntegration platformIntegration,
            II18nService i18n,
            IAlertService alertService,
            IAppFileSystem appFileSystem)
            : base(settingService, mediaResourceManager, platformIntegration, i18n, alertService, appFileSystem)
        {
        }

        public override async Task<string> SetAvatarByCaptureAsync()
        {
            await _alertService.ErrorAsync(_i18n.T("Not supported on the current platform"));
            return string.Empty;
        }
    }
}
