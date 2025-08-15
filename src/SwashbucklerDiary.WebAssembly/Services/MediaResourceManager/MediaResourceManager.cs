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
    }
}
