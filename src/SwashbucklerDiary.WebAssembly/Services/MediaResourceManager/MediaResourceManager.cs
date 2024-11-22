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
            IPopupServiceHelper popupServiceHelper,
            II18nService i18nService,
            ILogger<MediaResourceManager> logger,
            IJSRuntime jSRuntime,
            NavigationManager navigationManager) :
            base(platformIntegration, appFileSystem, popupServiceHelper, i18nService, logger)
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
                    await _appFileSystem.FileMoveAsync(sourceFilePath, targetFilePath);
                }
                else
                {
                    await _appFileSystem.FileCopyAsync(targetFilePath, sourceFilePath);
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
                await _popupServiceHelper.Error(_i18n.T("External files are not supported"));
                return false;
            }
        }

        public override async Task<bool> SaveFileAsync(string url)
        {
            if (IsStoredFile(url, out string filePath))
            {
                await _platformIntegration.SaveFileAsync(filePath);
                return true;
            }
            else
            {
                await _popupServiceHelper.Error(_i18n.T("External files are not supported"));
                return false;
            }
        }

        bool IsStoredFile(string url, out string filePath)
        {
            filePath = url.Replace(_navigationManager.BaseUri, "");
            return filePath.StartsWith(FileSystem.AppDataDirectory + "/");
        }

        public override string UrlRelativePathToFilePath(string urlRelativePath) => urlRelativePath;

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
