using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Shared;
using System.Text.RegularExpressions;

namespace SwashbucklerDiary.Rcl.Services
{
    public abstract class MediaResourceManager : IMediaResourceManager
    {
        protected readonly IPlatformIntegration _platformIntegration;

        protected readonly IAppFileManager _appFileManager;

        protected readonly IPopupServiceHelper _popupServiceHelper;

        protected readonly II18nService _i18n;

        protected readonly ILogger _logger;

        protected static readonly Dictionary<MediaResource, string> _mediaResourceFolders = new()
        {
            { MediaResource.Image, "Image" },
            { MediaResource.Audio, "Audio" },
            { MediaResource.Video, "Video" },
        };

        public Dictionary<MediaResource, string> MediaResourceFolders => _mediaResourceFolders;

        public MediaResourceManager(IPlatformIntegration mauiPlatformService,
            IAppFileManager appFileManager,
            IPopupServiceHelper popupServiceHelper,
            II18nService i18nService,
            ILogger<MediaResourceManager> logger)
        {
            _platformIntegration = mauiPlatformService;
            _appFileManager = appFileManager;
            _popupServiceHelper = popupServiceHelper;
            _i18n = i18nService;
            _logger = logger;
        }

        public async Task<ResourceModel?> AddAudioAsync()
        {
            string? filePath = await _platformIntegration.PickAudioAsync();
            return await AddMediaFileAsync(filePath);
        }

        public async Task<ResourceModel?> AddImageAsync()
        {
            string? filePath = await _platformIntegration.PickPhotoAsync();
            return await AddMediaFileAsync(filePath);
        }

        public async Task<ResourceModel?> AddVideoAsync()
        {
            string? filePath = await _platformIntegration.PickVideoAsync();
            return await AddMediaFileAsync(filePath);
        }

        private async Task<ResourceModel?> AddMediaFileAsync(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return null;
            }

            var kind = GetResourceKind(filePath);
            if (kind == MediaResource.Unknown)
            {
                return null;
            }

            string? uri = await CreateMediaResourceFileAsync(kind, filePath);
            if (string.IsNullOrEmpty(uri))
            {
                return null;
            }

            return new()
            {
                ResourceUri = uri,
                ResourceType = kind
            };
        }

        protected Task<string?> CreateMediaResourceFileAsync(MediaResource mediaResource, string? sourceFilePath)
        {
            var targetDirectoryPath = Path.Combine(_appFileManager.AppDataDirectory, MediaResourceFolders[mediaResource]);
            return CreateMediaResourceFileAsync(targetDirectoryPath, sourceFilePath);
        }

        public abstract Task<string?> CreateMediaResourceFileAsync(string targetDirectoryPath, string? sourceFilePath);

        public abstract Task<bool> ShareImageAsync(string title, string url);

        public abstract Task<bool> SaveImageAsync(string url);

        public List<ResourceModel> GetDiaryResources(string content)
        {
            var resources = new List<ResourceModel>();
            string pattern = $@"(?<=\(|"")({CustomPathPrefix}\S+?)(?=\)|"")";

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

        public abstract Task<AudioFileInfo> GetAudioFileInfo(string uri);

        public async Task<IEnumerable<ResourceModel>?> AddMediaFilesAsync(IEnumerable<string?>? filePaths)
        {
            if (filePaths is null)
            {
                return null;
            }

            List<ResourceModel> resources = [];
            foreach (var filePath in filePaths)
            {
                var resource = await AddMediaFileAsync(filePath);

                if (resource is null)
                {
                    continue;
                }

                resources.Add(resource);
            }

            return resources;
        }

        public abstract string UrlRelativePathToFilePath(string urlRelativePath);

        public async Task<IEnumerable<ResourceModel>?> AddMultipleImageAsync()
        {
            var filePaths = await _platformIntegration.PickMultiplePhotoAsync();
            return await AddMediaFilesAsync(filePaths);
        }

        public async Task<IEnumerable<ResourceModel>?> AddMultipleAudioAsync()
        {
            var filePaths = await _platformIntegration.PickMultipleAudioAsync();
            return await AddMediaFilesAsync(filePaths);
        }

        public async Task<IEnumerable<ResourceModel>?> AddMultipleVideoAsync()
        {
            var filePaths = await _platformIntegration.PickMultipleVideoAsync();
            return await AddMediaFilesAsync(filePaths);
        }

        protected virtual string? CustomPathPrefix { get; }
    }
}
