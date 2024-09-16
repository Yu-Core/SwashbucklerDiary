using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Maui.BlazorWebView;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Services
{
    public class MediaResourceManager : Rcl.Services.MediaResourceManager
    {
        private readonly HttpClient _httpClient;

        private readonly string _customPathPrefix = LocalFileWebAccessHelper.AppFilePathMap[FileSystem.AppDataDirectory];

        protected override string? CustomPathPrefix => _customPathPrefix;

        public MediaResourceManager(IPlatformIntegration platformIntegration,
            IAppFileManager appFileManager,
            IPopupServiceHelper popupServiceHelper,
            II18nService i18nService,
            ILogger<MediaResourceManager> logger)
            : base(platformIntegration, appFileManager, popupServiceHelper, i18nService, logger)
        {
            _httpClient = new HttpClient();
        }

        public override async Task<string?> CreateMediaResourceFileAsync(string targetDirectoryPath, string? sourceFilePath)
        {
            if (string.IsNullOrEmpty(sourceFilePath))
            {
                return null;
            }

            using Stream stream = File.OpenRead(sourceFilePath);
            var fn = stream.CreateMD5() + Path.GetExtension(sourceFilePath);
            var targetFilePath = Path.Combine(targetDirectoryPath, fn);

            if (!File.Exists(targetFilePath))
            {
                if (sourceFilePath.StartsWith(FileSystem.CacheDirectory))
                {
                    stream.Close();
                    await _appFileManager.FileMoveAsync(sourceFilePath, targetFilePath);
                }
                else
                {
                    //将流的位置重置为起始位置
                    stream.Seek(0, SeekOrigin.Begin);
                    await _appFileManager.FileCopyAsync(targetFilePath, stream);
                }
            }

            return LocalFileWebAccessHelper.FilePathToUrlRelativePath(targetFilePath);
        }

        public override async Task<bool> ShareImageAsync(string title, string url)
        {
            var filePath = await GetImageFilePathAsync(url);
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            await _platformIntegration.ShareFileAsync(title, filePath);
            return true;
        }

        public override async Task<bool> SaveImageAsync(string url)
        {
            var filePath = await GetImageFilePathAsync(url);
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            return await _platformIntegration.SaveFileAsync(filePath);
        }

        private async Task<string> GetImageFilePathAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }

            string? filePath;
            if (IsInternalUrl(url, out string relativePath))
            {
                filePath = LocalFileWebAccessHelper.UrlRelativePathToFilePath(relativePath);
                if (string.IsNullOrEmpty(filePath))
                {
                    filePath = await CopyPackageFileAndCreateTempFileAsync(url);
                }
            }
            else
            {
                filePath = await DownloadFileAndCreateTempFileAsync(url);
            }

            if (string.IsNullOrEmpty(filePath))
            {
                await _popupServiceHelper.Error(_i18n.T("Image.Not exist"));
            }

            return filePath;
        }

        bool IsInternalUrl(string uriString, out string relativePath)
        {
            string baseUriString = MauiBlazorWebViewHandler.BaseUri;
            string absoluteUri = new Uri(new Uri(baseUriString), uriString).ToString();
            if (absoluteUri.StartsWith(baseUriString))
            {
                relativePath = absoluteUri.Substring(baseUriString.Length);
                return true;
            }
            else
            {
                relativePath = string.Empty;
                return false;
            }
        }

        async Task<string> DownloadFileAndCreateTempFileAsync(string url)
        {
            string filePath = string.Empty;
            try
            {
                using Stream stream = await _httpClient.GetStreamAsync(url);
                var fileName = Path.GetFileName(url);
                filePath = await _appFileManager.CreateTempFileAsync(fileName, stream);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "DownloadFile fail");
            }

            return filePath;
        }

        async Task<string> CopyPackageFileAndCreateTempFileAsync(string url)
        {
            var exists = await FileSystem.AppPackageFileExistsAsync($"wwwroot/{url}");
            if (!exists)
            {
                return string.Empty;
            }

            using var stream = await FileSystem.OpenAppPackageFileAsync($"wwwroot/{url}");
            var fileName = Path.GetFileName(url);
            return await _appFileManager.CreateTempFileAsync(fileName, stream);
        }

        public override async Task<AudioFileInfo> GetAudioFileInfo(string uri)
        {
            string? filePath = LocalFileWebAccessHelper.UrlRelativePathToFilePath(uri);
            if (!File.Exists(filePath))
            {
                return new();
            }

            var audioFile = TagLib.File.Create(filePath);
            string pictureUri = string.Empty;
            if (audioFile.Tag.Pictures.Length > 0)
            {
                string fileName = Path.GetFileName(filePath);
                string extension = audioFile.Tag.Pictures[0].MimeType.Split('/')[1];
                string pictureFileName = $"{fileName}.{extension}";
                string pictureFilePath = FileSystem.Current.CacheDirectory + Path.DirectorySeparatorChar + pictureFileName;
                if (!File.Exists(pictureFilePath))
                {
                    await _appFileManager.CreateTempFileAsync(pictureFileName, audioFile.Tag.Pictures[0].Data.Data);
                }

                pictureUri = LocalFileWebAccessHelper.FilePathToUrlRelativePath(pictureFilePath);
            }

            return new()
            {
                Title = audioFile.Tag.Title,
                Artists = audioFile.Tag.Performers,
                Album = audioFile.Tag.Album,
                Duration = audioFile.Properties.Duration,
                PictureUri = pictureUri
            };
        }

        public override string UrlRelativePathToFilePath(string urlRelativePath)
            => LocalFileWebAccessHelper.UrlRelativePathToFilePath(urlRelativePath);
    }
}
