using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.WebAssembly.Services
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

            //bool isCaptureSupported = await _platformIntegration.IsCaptureSupported();
            //if (!isCaptureSupported)
            //{
            //    await _alertService.Error(_i18n.T("The current platform is unable to take photos"));
            //    return string.Empty;
            //}

            //string? photoPath = await _platformIntegration.CapturePhotoAsync();
            //if (string.IsNullOrEmpty(photoPath))
            //{
            //    return string.Empty;
            //}

            //return await SetAvatar(photoPath);
        }
    }
}
