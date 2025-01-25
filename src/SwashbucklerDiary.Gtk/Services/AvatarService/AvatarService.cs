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
            IPopupServiceHelper popupServiceHelper,
            IAppFileSystem appFileSystem)
            : base(settingService, mediaResourceManager, platformIntegration, i18n, popupServiceHelper, appFileSystem)
        {
        }

        public override async Task<string> SetAvatarByCapture()
        {
            await _popupServiceHelper.Error(_i18n.T("Share.NotSupported"));
            return string.Empty;
        }
    }
}
