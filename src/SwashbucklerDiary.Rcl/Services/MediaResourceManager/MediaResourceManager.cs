using Microsoft.AspNetCore.Components;
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

        protected readonly IAppFileSystem _appFileSystem;

        protected readonly II18nService _i18n;

        protected readonly ISettingService _settingService;

        protected readonly ILogger _logger;

        protected static readonly Dictionary<MediaResource, string> _mediaResourceFolders = new()
        {
            { MediaResource.Image, "Image" },
            { MediaResource.Audio, "Audio" },
            { MediaResource.Video, "Video" },
        };

        public Dictionary<MediaResource, string> MediaResourceFolders => _mediaResourceFolders;

        public virtual string? LinkBase => "";

        public MediaResourceManager(IPlatformIntegration mauiPlatformService,
            IAppFileSystem appFileSystem,
            II18nService i18nService,
            ISettingService settingService,
            ILogger<MediaResourceManager> logger)
        {
            _platformIntegration = mauiPlatformService;
            _appFileSystem = appFileSystem;
            _i18n = i18nService;
            _settingService = settingService;
            _logger = logger;
        }

        public async Task<ResourceModel?> AddAudioAsync()
        {
            string? filePath = await _platformIntegration.PickAudioAsync().ConfigureAwait(false);
            return await AddMediaFileAsync(filePath).ConfigureAwait(false);
        }

        public async Task<ResourceModel?> AddImageAsync()
        {
            string? filePath = await _platformIntegration.PickPhotoAsync().ConfigureAwait(false);
            return await AddMediaFileAsync(filePath).ConfigureAwait(false);
        }

        public async Task<ResourceModel?> AddVideoAsync()
        {
            string? filePath = await _platformIntegration.PickVideoAsync().ConfigureAwait(false);
            return await AddMediaFileAsync(filePath).ConfigureAwait(false);
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

            string? uri = await CreateMediaResourceFileAsync(kind, filePath).ConfigureAwait(false);
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
            var targetDirectoryPath = Path.Combine(_appFileSystem.AppDataDirectory, MediaResourceFolders[mediaResource]);
            return CreateMediaResourceFileAsync(targetDirectoryPath, sourceFilePath);
        }

        public async Task<string?> CreateMediaResourceFileAsync(string targetDirectoryPath, string? sourceFilePath)
        {
            if (string.IsNullOrEmpty(sourceFilePath))
            {
                return null;
            }

            bool useOriginalFileName = _settingService.Get(it => it.OriginalFileName);

            string targetFilePath;
            if (useOriginalFileName)
            {
                var fn = Path.GetFileName(sourceFilePath);
                string folderName = Guid.NewGuid().ToString("N");
                targetFilePath = Path.Combine(targetDirectoryPath, folderName, fn);
            }
            else
            {
                await using var stream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read,
                        bufferSize: 1024 * 1024, useAsync: true);
                string sha256 = await stream.CreateSHA256Async().ConfigureAwait(false);
                var fn = sha256 + Path.GetExtension(sourceFilePath);
                targetFilePath = Path.Combine(targetDirectoryPath, fn);
            }

            if (useOriginalFileName || !File.Exists(targetFilePath))
            {
                await Task.Run(() =>
                {
                    if (sourceFilePath.StartsWith(_appFileSystem.CacheDirectory))
                    {
                        _appFileSystem.FileMove(sourceFilePath, targetFilePath);
                    }
                    else
                    {
                        _appFileSystem.FileCopy(sourceFilePath, targetFilePath);
                    }
                }).ConfigureAwait(false);

                await _appFileSystem.SyncFS().ConfigureAwait(false);
            }

            return FilePathToRelativeUrl(targetFilePath);
        }

        public List<ResourceModel> GetDiaryResources(string content)
        {
            var resourceUris = new HashSet<string>();
            var resources = new List<ResourceModel>();
            string pattern = @$"(?<=\(|"")({customPathPrefix}.+?\.[a-zA-Z]+)(?=\)|"")";

            foreach (Match match in Regex.Matches(content, pattern))
            {
                string uri = match.Value;
                if (resourceUris.Add(uri))
                {
                    resources.Add(new()
                    {
                        ResourceType = GetResourceKind(uri),
                        ResourceUri = uri,
                    });
                }
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

        public async Task<AudioFileInfo> GetAudioFileInfo(string uri)
        {
            string? filePath = RelativeUrlToFilePath(uri);
            if (!File.Exists(filePath))
            {
                return new();
            }

            var audioFile = TagLib.File.Create(filePath);
            string? pictureUri = null;
            if (audioFile.Tag.Pictures.Length > 0)
            {
                string fileName = Path.GetFileName(filePath);
                string extension = StaticContentProvider.GetResponseExtensionOrDefault(audioFile.Tag.Pictures[0].MimeType);
                string pictureFileName = $"{fileName}{extension}";
                pictureUri = await GetAudioFilePicturePath(pictureFileName, audioFile.Tag.Pictures[0].Data.Data).ConfigureAwait(false);
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

        public abstract Task<string?> ToFilePathAsync(MediaResourcePath? mediaResourcePath);

        protected async Task<string?> GetAudioFilePicturePath(string fileName, byte[] data)
        {
            string filePath = Path.Combine(_appFileSystem.CacheDirectory, fileName);
            if (!File.Exists(filePath))
            {
                await File.WriteAllBytesAsync(filePath, data).ConfigureAwait(false);
                await _appFileSystem.SyncFS().ConfigureAwait(false);
            }

            var relativePath = FilePathToRelativeUrl(filePath);
            return relativePath;
        }

        public async Task<IEnumerable<ResourceModel>?> AddMediaFilesAsync(IEnumerable<string?>? filePaths)
        {
            if (filePaths is null)
            {
                return null;
            }

            List<ResourceModel> resources = [];
            foreach (var filePath in filePaths)
            {
                var resource = await AddMediaFileAsync(filePath).ConfigureAwait(false);

                if (resource is null)
                {
                    continue;
                }

                resources.Add(resource);
            }

            return resources;
        }

        public abstract string RelativeUrlToFilePath(string urlRelativePath);

        public abstract string FilePathToRelativeUrl(string filePath);

        public async Task<IEnumerable<ResourceModel>?> AddMultipleImageAsync()
        {
            var filePaths = await _platformIntegration.PickMultiplePhotoAsync().ConfigureAwait(false);
            return await AddMediaFilesAsync(filePaths).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ResourceModel>?> AddMultipleAudioAsync()
        {
            var filePaths = await _platformIntegration.PickMultipleAudioAsync().ConfigureAwait(false);
            return await AddMediaFilesAsync(filePaths).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ResourceModel>?> AddMultipleVideoAsync()
        {
            var filePaths = await _platformIntegration.PickMultipleVideoAsync().ConfigureAwait(false);
            return await AddMediaFilesAsync(filePaths).ConfigureAwait(false);
        }

        public async Task<string?> CreateMediaFilesInsertContentAsync(List<string?> filePaths)
        {
            var resources = await AddMediaFilesAsync(filePaths).ConfigureAwait(false);
            return CreateMediaFilesInsertContent(resources);
        }

        public string? CreateMediaFilesInsertContent(IEnumerable<ResourceModel>? resources)
        {
            if (resources is null) return null;
            var insertContents = resources.Select(it => CreateMediaFileInsertContent(it.ResourceUri!, it.ResourceType));
            if (insertContents is null || !insertContents.Any()) return null;
            return $"{string.Join("\n", insertContents)}\n\n";
        }

        protected static readonly string customPathPrefix = $"{AppFileSystem.AppDataVirtualDirectoryName}/";

        protected static string? CreateMediaFileInsertContent(string src, MediaResource mediaResource)
        {
            return mediaResource switch
            {
                MediaResource.Image => $"![]({src})",
                MediaResource.Audio => $"<audio src=\"{src}\" controls ></audio>",
                MediaResource.Video => $"<video src=\"{src}\" controls ></video>",
                _ => null
            };
        }

        public virtual string? ReplaceDisplayedUrlToRelativeUrl(string? content) => content;

        protected virtual string? RelativeUrlToDisplayedUrl(string? content) => content;

        public MediaResourcePath? ToMediaResourcePath(NavigationManager navigationManager, string? url)
        {
            url = ReplaceDisplayedUrlToRelativeUrl(url);

            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            var absoluteUri = navigationManager.ToAbsoluteUri(url).ToString();
            string? relativePath;

            try
            {
                relativePath = navigationManager.ToBaseRelativePath(absoluteUri);
            }
            catch (ArgumentException) // 捕获特定异常
            {
                relativePath = null; // 或设置降级值，如 url（原始URL）
            }

            return new MediaResourcePath
            {
                Url = absoluteUri,
                RelativePathOfBaseUri = relativePath,
                DisPlayedUrl = string.IsNullOrEmpty(relativePath) ? absoluteUri : RelativeUrlToDisplayedUrl(relativePath)
            };
        }
    }
}

