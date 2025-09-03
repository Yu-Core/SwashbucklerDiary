using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Maui.Services
{
    public class MediaResourceManager : Rcl.Hybird.Services.MediaResourceManager
    {
        public MediaResourceManager(IPlatformIntegration platformIntegration,
            IAppFileSystem appFileSystem,
            II18nService i18nService,
            ISettingService settingService,
            ILogger<MediaResourceManager> logger)
            : base(platformIntegration, appFileSystem, i18nService, settingService, logger)
        {
        }

        public override async Task<string?> ToFilePathAsync(MediaResourcePath? path)
        {
            string? filePath = await base.ToFilePathAsync(path).ConfigureAwait(false);
            if (string.IsNullOrEmpty(filePath) && path?.RelativePathOfBaseUri is string relativePath)
            {
                filePath = await CopyPackageFileAndCreateTempFileAsync(relativePath);
            }

            return filePath;
        }

        async Task<string> CopyPackageFileAndCreateTempFileAsync(string relativePath)
        {
            var exists = await FileSystem.AppPackageFileExistsAsync($"wwwroot/{relativePath}").ConfigureAwait(false);
            if (!exists)
            {
                return string.Empty;
            }

            using var stream = await FileSystem.OpenAppPackageFileAsync($"wwwroot/{relativePath}").ConfigureAwait(false);
            var fileName = Path.GetFileName(relativePath);
            return await _appFileSystem.CreateTempFileAsync(fileName, stream).ConfigureAwait(false);
        }
    }
}
