
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Services
{
    public abstract class AvatarService : IAvatarService
    {
        protected readonly ISettingService _settingService;

        protected readonly IMediaResourceManager _mediaResourceManager;

        protected readonly IPlatformIntegration _platformIntegration;

        protected readonly II18nService _i18n;

        protected readonly IAlertService _alertService;

        protected readonly IAppFileSystem _appFileSystem;

        protected readonly string targetDirectoryPath;

        public static string AvatarDirectoryName { get; } = "Avatar";

        public AvatarService(ISettingService settingService,
            IMediaResourceManager mediaResourceManager,
            IPlatformIntegration platformIntegration,
            II18nService i18n,
            IAlertService alertService,
            IAppFileSystem appFileSystem)
        {
            _settingService = settingService;
            _mediaResourceManager = mediaResourceManager;
            _platformIntegration = platformIntegration;
            _i18n = i18n;
            _alertService = alertService;
            _appFileSystem = appFileSystem;

            targetDirectoryPath = Path.Combine(_appFileSystem.AppDataDirectory, AvatarDirectoryName);
        }

        public abstract Task<string> SetAvatarByCapture();

        public async Task<string> SetAvatarByPickPhoto()
        {
            string? photoPath = await _platformIntegration.PickPhotoAsync();
            if (string.IsNullOrEmpty(photoPath))
            {
                return string.Empty;
            }

            return await SetAvatar(photoPath);
        }

        protected async Task<string> SetAvatar(string filePath)
        {
            string previousAvatarUri = _settingService.Get(s => s.Avatar);
            string previousAvatarPath = _mediaResourceManager.UrlRelativePathToFilePath(previousAvatarUri);
            if (File.Exists(previousAvatarPath))
            {
                File.Delete(previousAvatarPath);
            }

            string uri = await _mediaResourceManager.CreateMediaResourceFileAsync(targetDirectoryPath, filePath) ?? string.Empty;
            await _settingService.SetAsync(s => s.Avatar, uri);
            return uri;
        }
    }
}
