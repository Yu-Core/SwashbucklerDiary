using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Maui.BlazorWebView;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Maui.Services
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
                if (string.IsNullOrEmpty(filePath))
                {
                    filePath = await CopyPackageFileAndCreateTempFileAsync(relativePath).ConfigureAwait(false);
                }
            }
            else
            {
                filePath = await DownloadFileAndCreateTempFileAsync(urlString).ConfigureAwait(false);
            }

            if (string.IsNullOrEmpty(filePath))
            {
                await _alertService.ErrorAsync(_i18n.T("File does not exist"));
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
                using Stream stream = await _httpClient.GetStreamAsync(url).ConfigureAwait(false);
                var fileName = Path.GetFileName(url);
                filePath = await _appFileSystem.CreateTempFileAsync(fileName, stream).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "DownloadFile fail");
            }

            return filePath;
        }

        async Task<string> CopyPackageFileAndCreateTempFileAsync(string url)
        {
            var exists = await FileSystem.AppPackageFileExistsAsync($"wwwroot/{url}").ConfigureAwait(false);
            if (!exists)
            {
                return string.Empty;
            }

            using var stream = await FileSystem.OpenAppPackageFileAsync($"wwwroot/{url}").ConfigureAwait(false);
            var fileName = Path.GetFileName(url);
            return await _appFileSystem.CreateTempFileAsync(fileName, stream).ConfigureAwait(false);
        }

        public override string UrlRelativePathToFilePath(string urlRelativePath)
            => LocalFileWebAccessHelper.UrlRelativePathToFilePath(urlRelativePath);

        public override string FilePathToUrlRelativePath(string filePath)
            => LocalFileWebAccessHelper.FilePathToUrlRelativePath(filePath);
    }
}
