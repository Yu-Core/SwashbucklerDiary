using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class AvatarService : Rcl.Services.AvatarService
    {
        private readonly string targetDirectoryPath = Path.Combine(FileSystem.AppDataDirectory, avatarDirectoryName);

        public AvatarService(ISettingService settingService,
            IMediaResourceManager mediaResourceManager,
            IPlatformIntegration platformIntegration,
            II18nService i18n,
            IAlertService alertService)
            : base(settingService, mediaResourceManager, platformIntegration, i18n, alertService)
        {
        }

        protected override async Task<string> SetAvatar(string filePath)
        {
            string previousAvatarUri = _settingService.Get<string>(Setting.Avatar);
            if (File.Exists(previousAvatarUri))
            {
                File.Delete(previousAvatarUri);
            }

            string uri = await _mediaResourceManager.CreateMediaResourceFileAsync(targetDirectoryPath, filePath) ?? string.Empty;
            await _settingService.Set(Setting.Avatar, uri);
            return uri;
        }

        public override async Task<string> SetAvatarByCapture()
        {
            bool isCaptureSupported = await _platformIntegration.IsCaptureSupported();
            if (!isCaptureSupported)
            {
                await _alertService.Error(_i18n.T("User.NoCapture"));
                return string.Empty;
            }

            string? photoPath = await _platformIntegration.CapturePhotoAsync();
            if (string.IsNullOrEmpty(photoPath))
            {
                return string.Empty;
            }

            return await SetAvatar(photoPath);
        }
    }
}
