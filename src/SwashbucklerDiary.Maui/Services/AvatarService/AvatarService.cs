using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Maui.Services
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
            bool isCaptureSupported = await _platformIntegration.IsCaptureSupported();
            if (!isCaptureSupported)
            {
                await _alertService.ErrorAsync(_i18n.T("The current platform is unable to take photos"));
                return string.Empty;
            }

            var cameraPermission = await _platformIntegration.TryCameraPermission();
            if (!cameraPermission)
            {
                await _alertService.InfoAsync(_i18n.T("Please grant permission for the camera"));
                return string.Empty;
            }

            var writePermission = await _platformIntegration.TryStorageWritePermission();
            if (!writePermission)
            {
                await _alertService.InfoAsync(_i18n.T("Please grant permission for storage writing"));
                return string.Empty;
            }

            string? photoPath = await _platformIntegration.CapturePhotoAsync();
            if (string.IsNullOrEmpty(photoPath))
            {
                return string.Empty;
            }

            return await SetAvatarAsync(photoPath);
        }
    }
}
