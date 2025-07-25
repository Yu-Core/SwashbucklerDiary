using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Gtk.BlazorWebView;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Gtk.Services
{
    public class MediaResourceManager : Rcl.Services.MediaResourceManager
    {
        private readonly HttpClient _httpClient;

        public MediaResourceManager(IPlatformIntegration platformIntegration,
            IAppFileSystem appFileSystem,
            IAlertService alertService,
            II18nService i18nService,
            ILogger<MediaResourceManager> logger)
            : base(platformIntegration, appFileSystem, alertService, i18nService, logger)
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

        protected override async Task<string> GetResourceFilePathAsync(string? urlString)
        {
            if (string.IsNullOrEmpty(urlString))
            {
                return string.Empty;
            }

            string? filePath;
            if (IsInternalUrl(urlString, out string relativePath))
            {
                filePath = LocalFileWebAccessHelper.UrlRelativePathToFilePath(relativePath);
            }
            else
            {
                filePath = await DownloadFileAndCreateTempFileAsync(urlString);
            }

            if (string.IsNullOrEmpty(filePath))
            {
                await _alertService.Error(_i18n.T("File does not exist"));
            }

            return filePath;
        }

        bool IsInternalUrl(string uriString, out string relativePath)
        {
            string baseUriString = GtkWebViewManagerHelper.AppOriginUri.ToString();
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


        public override string UrlRelativePathToFilePath(string urlRelativePath)
            => LocalFileWebAccessHelper.UrlRelativePathToFilePath(urlRelativePath);

        public override string FilePathToUrlRelativePath(string filePath)
            => LocalFileWebAccessHelper.FilePathToUrlRelativePath(filePath);
    }
}
