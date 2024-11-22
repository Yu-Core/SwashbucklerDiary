using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Maui.BlazorWebView;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Services
{
    public class MediaResourceManager : Rcl.Services.MediaResourceManager
    {
        private readonly HttpClient _httpClient;

        public MediaResourceManager(IPlatformIntegration platformIntegration,
            IAppFileSystem appFileSystem,
            IPopupServiceHelper popupServiceHelper,
            II18nService i18nService,
            ILogger<MediaResourceManager> logger)
            : base(platformIntegration, appFileSystem, popupServiceHelper, i18nService, logger)
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
                    await _appFileSystem.FileMoveAsync(sourceFilePath, targetFilePath);
                }
                else
                {
                    //将流的位置重置为起始位置
                    stream.Seek(0, SeekOrigin.Begin);
                    await _appFileSystem.FileCopyAsync(targetFilePath, stream);
                }
            }

            return LocalFileWebAccessHelper.FilePathToUrlRelativePath(targetFilePath);
        }

        public override async Task<bool> ShareImageAsync(string title, string url)
        {
            var filePath = await GetResourceFilePathAsync(url);
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            await _platformIntegration.ShareFileAsync(title, filePath);
            return true;
        }

        public override async Task<bool> SaveFileAsync(string url)
        {
            var filePath = await GetResourceFilePathAsync(url);
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            return await _platformIntegration.SaveFileAsync(filePath);
        }

        private async Task<string> GetResourceFilePathAsync(string? url)
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
                await _popupServiceHelper.Error(_i18n.T("FileBrowse.Not exist"));
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
                filePath = await _appFileSystem.CreateTempFileAsync(fileName, stream);
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
            return await _appFileSystem.CreateTempFileAsync(fileName, stream);
        }

        public override string UrlRelativePathToFilePath(string urlRelativePath)
            => LocalFileWebAccessHelper.UrlRelativePathToFilePath(urlRelativePath);

        protected override async Task<string> GetAudioFilePicturePath(string fileName, byte[] data)
        {
            string filePath = Path.Combine(FileSystem.Current.CacheDirectory, fileName);
            if (!File.Exists(filePath))
            {
                await File.WriteAllBytesAsync(filePath, data);
            }

            return LocalFileWebAccessHelper.FilePathToUrlRelativePath(filePath);
        }
    }
}
