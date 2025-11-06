using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Hybird.Services
{
    public abstract class MediaResourceManager : Rcl.Services.MediaResourceManager, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly LocalFileWebAssetServer localFileWebAssetServer;

        protected MediaResourceManager(IPlatformIntegration mauiPlatformService,
            IAppFileSystem appFileSystem,
            II18nService i18nService,
            ISettingService settingService,
            ILogger<Rcl.Services.MediaResourceManager> logger) : base(mauiPlatformService, appFileSystem, i18nService, settingService, logger)
        {
            _httpClient = new HttpClient();

            localFileWebAssetServer = new(routeFilePathMap);
        }

        public override string? MarkdownLinkBase => localFileWebAssetServer?.UrlPrefix;

        public override async Task<string?> ToFilePathAsync(MediaResourcePath? path)
        {
            if (path is null)
            {
                return null;
            }

            string? filePath = null;
            if (path.RelativePathOfBaseUri is string relativePath)
            {
                filePath = RelativeUrlToFilePath(relativePath);
            }
            else if (path.Url is string url)
            {
                filePath = await DownloadFileAndCreateTempFileAsync(url).ConfigureAwait(false);
            }

            return filePath;
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

        public override string? ReplaceDisplayedUrlToRelativeUrl(string? content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return string.Empty;
            }

            return content.Replace($"{localFileWebAssetServer.UrlPrefix}/", "");
        }

        protected override string? RelativeUrlToDisplayedUrl(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                return null;
            }

            var normalizedUrl = $"/{relativePath.Trim('/')}";
            if (routeFilePathMap.Keys.Any(it => normalizedUrl.StartsWith($"{it}/", StringComparison.OrdinalIgnoreCase)))
            {
                return $"{localFileWebAssetServer.UrlPrefix}/{relativePath}";
            }

            return relativePath;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient.Dispose();
                localFileWebAssetServer.Dispose();
            }
        }
    }
}
