using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Hybird.Services
{
    public abstract class MediaResourceManager : Rcl.Services.MediaResourceManager, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly LocalFileWebAssetServer localFileWebAssetServer;
        private readonly Dictionary<string, string> _routeFileSystemPathMap;

        protected MediaResourceManager(IPlatformIntegration mauiPlatformService,
            IAppFileSystem appFileSystem,
            II18nService i18nService,
            ISettingService settingService,
            ILogger<Rcl.Services.MediaResourceManager> logger) : base(mauiPlatformService, appFileSystem, i18nService, settingService, logger)
        {
            _httpClient = new HttpClient();

            _routeFileSystemPathMap = new()
            {
                { $"/{AppFileSystem.AppDataVirtualDirectoryName}", appFileSystem.AppDataDirectory },
                { $"/{AppFileSystem.CacheVirtualDirectoryName}", appFileSystem.CacheDirectory },
            };

            localFileWebAssetServer = new(_routeFileSystemPathMap);
        }

        public override string? LinkBase => localFileWebAssetServer?.UrlPrefix;

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

        // 将真实的文件路径转化为 URL 相对路径
        public override string FilePathToRelativeUrl(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return string.Empty;
            }

            // 查找匹配的最长路径前缀
            var match = _routeFileSystemPathMap
                .Where(pair => filePath.StartsWith(pair.Value, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(pair => pair.Value.Length)
                .FirstOrDefault();

            if (match.Equals(default(KeyValuePair<string, string>)))
            {
                return string.Empty;
            }

            // 获取相对路径部分并转换分隔符
            var relativePath = filePath.Substring(match.Value.Length)
                .TrimStart(Path.DirectorySeparatorChar)
                .Replace(Path.DirectorySeparatorChar, '/');

            // 组合URL并确保不以/开头
            return $"{match.Key.TrimStart('/')}/{relativePath}".TrimEnd('/');
        }

        // 将相对URL转换为文件路径
        public override string RelativeUrlToFilePath(string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
            {
                return string.Empty;
            }

            // 标准化URL输入
            var normalizedUrl = $"{Uri.UnescapeDataString(relativeUrl).Trim('/')}";

            // 查找匹配的最长路由前缀
            var match = _routeFileSystemPathMap
                .Where(pair => normalizedUrl.StartsWith($"{pair.Key.TrimStart('/')}/", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(pair => pair.Key.Length)
                .FirstOrDefault();

            if (match.Equals(default(KeyValuePair<string, string>)))
            {
                return string.Empty;
            }

            // 获取URL剩余部分并转换分隔符
            var remainingPath = normalizedUrl.Substring(match.Key.Length)
                .TrimStart('/')
                .Replace('/', Path.DirectorySeparatorChar);

            // 组合文件路径
            return Path.Combine(match.Value, remainingPath);
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
            if (_routeFileSystemPathMap.Keys.Any(it => normalizedUrl.StartsWith($"{it}/", StringComparison.OrdinalIgnoreCase)))
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
