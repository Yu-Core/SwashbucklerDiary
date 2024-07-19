using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class MediaResourceManager : Rcl.Services.MediaResourceManager
    {
        private readonly string _customPathPrefix = FileSystem.AppDataDirectory + "/";

        private readonly IJSRuntime _jSRuntime;

        protected override string? CustomPathPrefix => _customPathPrefix;

        private readonly NavigationManager _navigationManager;

        public MediaResourceManager(IPlatformIntegration platformIntegration,
            IAppFileManager appFileManager,
            IPopupServiceHelper popupServiceHelper,
            II18nService i18nService,
            ILogger<MediaResourceManager> logger,
            IJSRuntime jSRuntime,
            NavigationManager navigationManager) :
            base(platformIntegration, appFileManager, popupServiceHelper, i18nService, logger)
        {
            _jSRuntime = jSRuntime;
            _navigationManager = navigationManager;
        }

        protected override Task<string?> CreateMediaResourceFileAsync(MediaResource mediaResource, string? sourceFilePath)
        {
            var targetDirectoryPath = Path.Combine(FileSystem.AppDataDirectory, MediaResourceFolders[mediaResource]);
            return CreateMediaResourceFileAsync(targetDirectoryPath, sourceFilePath);
        }

        public override async Task<string?> CreateMediaResourceFileAsync(string targetDirectoryPath, string? sourceFilePath)
        {
            if (string.IsNullOrEmpty(sourceFilePath))
            {
                return null;
            }

            var fn = Path.GetFileName(sourceFilePath);
            var targetFilePath = Path.Combine(targetDirectoryPath, fn);

            if (!File.Exists(targetFilePath))
            {
                if (sourceFilePath.StartsWith(FileSystem.CacheDirectory))
                {
                    await _appFileManager.FileMoveAsync(sourceFilePath, targetFilePath);
                }
                else
                {
                    await _appFileManager.FileCopyAsync(targetFilePath, sourceFilePath);
                }

                //由于设置的从memfs(内存)到idbfs(indexedDB)的同步时间为1s，拦截请求(service worker)那里会找不到文件，所以此处应立即同步
                await _jSRuntime.InvokeVoidAsync("MEMFileSystem.syncfs");
            }

            return targetFilePath;
        }

        public override async Task<bool> ShareImageAsync(string title, string url)
        {
            if (IsStoredFile(url, out string filePath))
            {
                await _platformIntegration.ShareFileAsync(title, filePath);
                return true;
            }
            else
            {
                await _popupServiceHelper.Error(_i18n.T("External images are not supported"));
                return false;
            }
        }

        public override async Task<bool> SaveImageAsync(string url)
        {
            if (IsStoredFile(url, out string filePath))
            {
                await _platformIntegration.SaveFileAsync(filePath);
                return true;
            }
            else
            {
                await _popupServiceHelper.Error(_i18n.T("External images are not supported"));
                return false;
            }
        }

        bool IsStoredFile(string url, out string filePath)
        {
            filePath = url.Replace(_navigationManager.BaseUri, "");
            return filePath.StartsWith(FileSystem.AppDataDirectory + "/");
        }

        public override async Task<AudioFileInfo> GetAudioFileInfo(string uri)
        {
            string? filePath = uri;
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
                string pictureFilePath = FileSystem.CacheDirectory + Path.DirectorySeparatorChar + pictureFileName;
                if (!File.Exists(pictureFilePath))
                {
                    await _appFileManager.CreateTempFileAsync(pictureFileName, audioFile.Tag.Pictures[0].Data.Data);
                    await _jSRuntime.InvokeVoidAsync("MEMFileSystem.syncfs");
                }

                pictureUri = pictureFilePath;
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

        public override string UrlRelativePathToFilePath(string urlRelativePath) => urlRelativePath;
    }
}
