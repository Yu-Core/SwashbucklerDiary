﻿
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

        protected readonly static string avatarDirectoryName = "Avatar";

        public AvatarService(ISettingService settingService,
            IMediaResourceManager mediaResourceManager,
            IPlatformIntegration platformIntegration,
            II18nService i18n,
            IAlertService alertService)
        {
            _settingService = settingService;
            _mediaResourceManager = mediaResourceManager;
            _platformIntegration = platformIntegration;
            _i18n = i18n;
            _alertService = alertService;
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

        protected abstract Task<string> SetAvatar(string filePath);


    }
}
