
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public abstract class AvatarService : IAvatarService
    {
        protected readonly ISettingService _settingService;

        protected readonly IMediaResourceManager _mediaResourceManager;

        protected readonly IPlatformIntegration _platformIntegration;

        protected readonly II18nService _i18n;

        protected readonly IPopupServiceHelper _popupServiceHelper;

        protected readonly IAppFileManager _appFileManager;

        protected readonly static string avatarDirectoryName = "Avatar";

        protected readonly string targetDirectoryPath;

        public AvatarService(ISettingService settingService,
            IMediaResourceManager mediaResourceManager,
            IPlatformIntegration platformIntegration,
            II18nService i18n,
            IPopupServiceHelper popupServiceHelper,
            IAppFileManager appFileManager)
        {
            _settingService = settingService;
            _mediaResourceManager = mediaResourceManager;
            _platformIntegration = platformIntegration;
            _i18n = i18n;
            _popupServiceHelper = popupServiceHelper;
            _appFileManager = appFileManager;

            targetDirectoryPath = Path.Combine(_appFileManager.AppDataDirectory, avatarDirectoryName);
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
            string previousAvatarUri = _settingService.Get<string>(Setting.Avatar);
            string previousAvatarPath = _mediaResourceManager.UrlRelativePathToFilePath(previousAvatarUri);
            if (File.Exists(previousAvatarPath))
            {
                File.Delete(previousAvatarPath);
            }

            string uri = await _mediaResourceManager.CreateMediaResourceFileAsync(targetDirectoryPath, filePath) ?? string.Empty;
            await _settingService.Set(Setting.Avatar, uri);
            return uri;
        }
    }
}
