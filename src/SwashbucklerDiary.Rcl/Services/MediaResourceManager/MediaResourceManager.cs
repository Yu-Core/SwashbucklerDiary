using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;
using System.Text.RegularExpressions;

namespace SwashbucklerDiary.Rcl.Services
{
    public abstract class MediaResourceManager : IMediaResourceManager
    {
        protected readonly IPlatformIntegration _platformIntegration;

        protected readonly IAppFileManager _appFileManager;

        protected readonly IAlertService _alertService;

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
            IAlertService alertService,
            II18nService i18nService,
            ILogger<MediaResourceManager> logger)
        {
            _platformIntegration = mauiPlatformService;
            _appFileManager = appFileManager;
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

        protected abstract Task<string?> CreateMediaResourceFileAsync(MediaResource mediaResource, string? sourceFilePath);

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

        protected virtual string? CustomPathPrefix { get; }


    }
}
