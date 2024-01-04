﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class MediaResourceManager : Rcl.Services.MediaResourceManager
    {
        private readonly string _customPathPrefix = FileSystem.AppDataDirectory + "/";

        private readonly Lazy<ValueTask<IJSObjectReference>> _module;

        protected override string? CustomPathPrefix => _customPathPrefix;

        private readonly NavigationManager _navigationManager;

        public MediaResourceManager(IPlatformIntegration platformIntegration,
            IAppFileManager appFileManager,
            IAlertService alertService,
            II18nService i18nService,
            ILogger<MediaResourceManager> logger,
            IJSRuntime jSRuntime,
            NavigationManager navigationManager) :
            base(platformIntegration, appFileManager, alertService, i18nService, logger)
        {
            _module = new(() => jSRuntime.ImportJsModule("js/fileSystem.js"));
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
                var module = await _module.Value;
                await module.InvokeVoidAsync("syncfs");
            }

            return targetFilePath;
        }

        public override async Task<bool> ShareImageAsync(string title, string url)
        {
            if (IsStoredFile(url,out string filePath))
            {
                await _platformIntegration.ShareFileAsync(title, filePath);
                return true;
            }
            else
            {
                await _alertService.Error(_i18n.T("External images are not supported"));
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
                await _alertService.Error(_i18n.T("External images are not supported"));
                return false;
            }
        }

        bool IsStoredFile (string url, out string filePath)
        {
            filePath = url.Replace(_navigationManager.BaseUri, "");
            return filePath.StartsWith(FileSystem.AppDataDirectory + "/");
        }
    }
}