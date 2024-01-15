using SwashbucklerDiary.Maui.BlazorWebView;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Services
{
    public class AvatarService : Rcl.Services.AvatarService
    {
        private readonly string targetDirectoryPath = Path.Combine(FileSystem.AppDataDirectory, avatarDirectoryName);

        public AvatarService(Rcl.Essentials.IPreferences preferences, 
            IMediaResourceManager mediaResourceManager, 
            IPlatformIntegration platformIntegration, 
            II18nService i18n, 
            IAlertService alertService) : base(preferences, mediaResourceManager, platformIntegration, i18n, alertService)
        {
        }

        protected override async Task<string> SetAvatar(string filePath)
        {
            string previousAvatarUri = await _preferences.Get<string>(Setting.Avatar); 
            string previousAvatarPath = MauiBlazorWebViewHandler.UrlRelativePathToFilePath(previousAvatarUri);
            if (!string.IsNullOrEmpty(previousAvatarPath))
            {
                File.Delete(previousAvatarPath);
            }
            string uri = await _mediaResourceManager.CreateMediaResourceFileAsync(targetDirectoryPath, filePath) ?? string.Empty;
            await _preferences.Set(Setting.Avatar, uri);
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

            var cameraPermission = await _platformIntegration.TryCameraPermission();
            if (!cameraPermission)
            {
                await _alertService.Info(_i18n.T("Permission.OpenCamera"));
                return string.Empty;
            }

            var writePermission = await _platformIntegration.TryStorageWritePermission();
            if (!writePermission)
            {
                await _alertService.Info(_i18n.T("Permission.OpenStorageWrite"));
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
