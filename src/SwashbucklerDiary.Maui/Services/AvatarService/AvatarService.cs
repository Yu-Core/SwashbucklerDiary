using DocumentFormat.OpenXml.Wordprocessing;
using SwashbucklerDiary.Maui.BlazorWebView;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Services
{
    public class AvatarService : IAvatarService
    {
        private readonly Rcl.Essentials.IPreferences _preferences;

        private readonly IMediaResourceManager _mediaResourceManager;

        private readonly string targetDirectoryPath = Path.Combine(FileSystem.AppDataDirectory, "Avatar");

        public AvatarService(Rcl.Essentials.IPreferences preferences,
            IMediaResourceManager mediaResourceManager)
        {
            _preferences = preferences;
            _mediaResourceManager = mediaResourceManager;
        }

        public async Task<string> SetAvatar(string filePath)
        {
            string previousAvatarUri = await _preferences.Get<string>(Setting.Avatar); 
            string previousAvatarPath = MauiBlazorWebViewHandler.UrlRelativePathToFilePath(previousAvatarUri);
            if (!string.IsNullOrEmpty(previousAvatarPath))
            {
                File.Delete(previousAvatarPath);
            }
            string uri = await ((MediaResourceManager)_mediaResourceManager).CreateMediaResourceFileAsync(targetDirectoryPath, filePath) ?? string.Empty;
            await _preferences.Set(Setting.Avatar, uri);
            return uri;
        }
    }
}
