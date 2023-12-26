using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Maui.BlazorWebView;
using SwashbucklerDiary.Maui.Extensions;
using SwashbucklerDiary.Maui.Utilities;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Text.RegularExpressions;

namespace SwashbucklerDiary.Maui.Services
{
    public class MediaResourceManager : IMediaResourceManager
    {
        private readonly IPlatformIntegration _platformIntegration;

        private readonly IAppFileManager _appFileManager;

        private readonly HttpClient _httpClient;

        private readonly IAlertService _alertService;

        private readonly II18nService _i18n;

        private readonly ILogger _logger;

        private readonly string customPathPrefix = MauiBlazorWebViewHandler.AppFilePathMap[FileSystem.AppDataDirectory] + "/";

        private static readonly Dictionary<MediaResource, string> _mediaResourceFolders = new()
        {
            { MediaResource.Image, "Image" },
            { MediaResource.Audio, "Audio" },
            { MediaResource.Video, "Video" },
        };

        public Dictionary<MediaResource, string> MediaResourceFolders => _mediaResourceFolders;

        public MediaResourceManager(IPlatformIntegration mauiPlatformService, 
            IAppFileManager appFileManager,
            IAlertService alertService,
            II18nService i18nService,
            ILogger<MediaResourceManager> logger)
        {
            _platformIntegration = mauiPlatformService;
            _appFileManager = appFileManager;
            _httpClient = new HttpClient();
            _alertService = alertService;
            _i18n = i18nService;
            _logger = logger;
        }

        public async Task<string?> AddAudioAsync()
        {
            string? pickPath = await _platformIntegration.PickAudioAsync();
            return await CreateMediaResourceFileAsync(MediaResource.Audio, pickPath);
        }

        public async Task<string?> AddImageAsync()
        {
            string? pickPath = await _platformIntegration.PickPhotoAsync();
            return await CreateMediaResourceFileAsync(MediaResource.Image, pickPath);
        }

        public async Task<string?> AddVideoAsync()
        {
            string? pickPath = await _platformIntegration.PickVideoAsync();
            return await CreateMediaResourceFileAsync(MediaResource.Video, pickPath);
        }

        private Task<string?> CreateMediaResourceFileAsync(MediaResource mediaResource, string? sourceFilePath)
        {
            var targetDirectoryPath = Path.Combine(FileSystem.AppDataDirectory, MediaResourceFolders[mediaResource]);
            return CreateMediaResourceFileAsync(targetDirectoryPath, sourceFilePath);
        }

        public async Task<string?> CreateMediaResourceFileAsync(string targetDirectoryPath, string? sourceFilePath)
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

            return MauiBlazorWebViewHandler.FilePathToUrlRelativePath(targetFilePath);
        }

        public async Task<bool> ShareImageAsync(string title, string url)
        {
            var filePath = await GetImageFilePathAsync(url);
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            await _platformIntegration.ShareFileAsync(title, filePath);
            return true;
        }

        public async Task<bool> SaveImageAsync(string url)
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
            if(string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }

            string? filePath;
            if (IsUrlBasedOnBaseUrl(url))
            {
                filePath = MauiBlazorWebViewHandler.UrlRelativePathToFilePath(url);
                if (string.IsNullOrEmpty(filePath))
                {
                    filePath = await CopyPackageFileAndCreateTempFileAsync(url);
                }
            }
            else
            {
                filePath = await DownloadFileAndCreateTempFileAsync(url);
            }
            
            if(string.IsNullOrEmpty(filePath))
            {
                await _alertService.Error(_i18n.T("Image.Not exist"));
            }

            return filePath;
        }

        bool IsUrlBasedOnBaseUrl(string url)
        {
            string baseUrl = MauiBlazorWebViewHandler.BaseUri;
            Uri baseUri = new Uri(baseUrl);
            Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);
            return baseUri.IsBaseOf(uri);
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

        public List<ResourceModel> GetDiaryResources(string content)
        {
            var resources = new List<ResourceModel>();
            string pattern = $@"(?<=\(|"")({customPathPrefix}\S+?)(?=\)|"")";

            MatchCollection matches = Regex.Matches(content, pattern);

            foreach (Match match in matches.Cast<Match>())
            {
                resources.Add(new()
                {
                    ResourceType = GetResourceKind(match.Value),
                    ResourceUri = match.Value,
                });
            }

            return resources;
        }

        public MediaResource GetResourceKind(string uri)
        {
            var mime = StaticContentProvider.GetResponseContentTypeOrDefault(uri);
            var type = mime.Split('/')[0];

            return type switch
            {
                "image" => MediaResource.Image,
                "audio" => MediaResource.Audio,
                "video" => MediaResource.Video,
                _ => MediaResource.Unknown
            };
        }
    }
}
