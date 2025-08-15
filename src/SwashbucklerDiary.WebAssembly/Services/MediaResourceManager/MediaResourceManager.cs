using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class MediaResourceManager : Rcl.Services.MediaResourceManager
    {
        private readonly IJSRuntime _jSRuntime;

        private readonly NavigationManager _navigationManager;

        public MediaResourceManager(IPlatformIntegration platformIntegration,
            IAppFileSystem appFileSystem,
            IAlertService alertService,
            II18nService i18nService,
            ILogger<MediaResourceManager> logger,
            IJSRuntime jSRuntime,
            NavigationManager navigationManager) :
            base(platformIntegration, appFileSystem, alertService, i18nService, logger)
        {
            _jSRuntime = jSRuntime;
            _navigationManager = navigationManager;
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
                    _appFileSystem.FileMove(sourceFilePath, targetFilePath);
                }
                else
                {
                    _appFileSystem.FileCopy(sourceFilePath, targetFilePath);
                }

                //由于设置的从memfs(内存)到idbfs(indexedDB)的同步时间为1s，拦截请求(service worker)那里会找不到文件，所以此处应立即同步
                await _jSRuntime.InvokeVoidAsync("MEMFileSystem.syncfs");
            }

            return targetFilePath;
        }

        bool IsStoredFile(string url, out string filePath)
        {
            filePath = url.Replace(_navigationManager.BaseUri, "");
            return filePath.StartsWith(FileSystem.AppDataDirectory + "/");
        }

        public override string UrlRelativePathToFilePath(string urlRelativePath) => urlRelativePath;

        public override string FilePathToUrlRelativePath(string filePath) => filePath;

        protected override async Task<string> GetResourceFilePathAsync(string? urlString)
        {
            if (string.IsNullOrEmpty(urlString))
            {
                return string.Empty;
            }


            if (IsStoredFile(urlString, out string filePath))
            {
                return filePath;
            }
            else
            {
                await _alertService.ErrorAsync(_i18n.T("External files are not supported"));
                return string.Empty;
            }
        }

        protected override async Task<string> GetAudioFilePicturePath(string fileName, byte[] data)
        {
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            if (!File.Exists(filePath))
            {
                await File.WriteAllBytesAsync(filePath, data);
                await _jSRuntime.InvokeVoidAsync("MEMFileSystem.syncfs");
            }

            return filePath;
        }
    }
}
