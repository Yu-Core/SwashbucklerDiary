﻿using SwashbucklerDiary.Rcl.Essentials;
using System.Net.Http.Json;

namespace SwashbucklerDiary.Rcl.Services
{
    public abstract class VersionUpdataManager : IVersionUpdataManager
    {
        public event Func<Task>? AfterFirstEnter;

        public event Func<Task>? AfterVersionUpdate;

        public event Func<Task>? AfterCheckFirstLaunch;

        protected int updateCount;

        protected readonly IDiaryService _diaryService;

        protected readonly IResourceService _resourceService;

        protected readonly ISettingService _settingService;

        protected readonly IMediaResourceManager _mediaResourceManager;

        protected HttpClient httpClient;

        protected readonly II18nService _i18n;

        protected readonly IVersionTracking _versionTracking;

        protected readonly IDiaryFileManager _diaryFileManager;

        private class Release
        {
            public string? Tag_Name { get; set; }
        }

        public VersionUpdataManager(IDiaryService diaryService,
            IResourceService resourceService,
            ISettingService settingService,
            IMediaResourceManager mediaResourceManager,
            II18nService i18n,
            IVersionTracking versionTracking,
            IDiaryFileManager diaryFileManager)
        {
            _diaryService = diaryService;
            _resourceService = resourceService;
            _settingService = settingService;
            _mediaResourceManager = mediaResourceManager;
            httpClient = new HttpClient();
            _i18n = i18n;
            _versionTracking = versionTracking;
            _diaryFileManager = diaryFileManager;
        }

        public async Task NotifyAfterFirstEnter()
        {
            if (AfterFirstEnter is null)
            {
                return;
            }

            await AfterFirstEnter.Invoke();
        }

        public virtual async Task HandleVersionUpdate()
        {
            if (AfterVersionUpdate is not null && updateCount > 0)
            {
                await AfterVersionUpdate.Invoke();
            }
        }

        public async Task<bool> CheckForUpdates()
        {
            using HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)");
            var release = await httpClient.GetFromJsonAsync<Release>(_i18n.T("VersionUpdate.LatestVersionUrl"));
            if (release is null || release.Tag_Name is null)
            {
                throw new Exception();
            }

            var latestVersion = new Version(release.Tag_Name.TrimStart('v'));
            var currentVersion = new Version(_versionTracking.CurrentVersion);
            if (latestVersion.CompareTo(currentVersion) > 0)
            {
                return true;
            }

            return false;
        }

        public abstract Task ToUpdate();

        protected async Task HandleVersionUpdate(string version, Func<Task> func)
        {
            string? strPreviousVersion = _versionTracking.PreviousVersion;
            if (string.IsNullOrEmpty(strPreviousVersion))
            {
                return;
            }

            var previousVersion = new Version(strPreviousVersion);
            var targetVersion = new Version(version);
            int result = previousVersion.CompareTo(targetVersion);
            if (result > 0)
            {
                return;
            }

            bool first = _versionTracking.IsFirstLaunchForCurrentVersion;
            if (!first)
            {
                return;
            }

            updateCount++;
            await func.Invoke();
        }

        protected abstract Task HandleVersionUpdate697();

        public async Task NotifyAfterCheckFirstLaunch()
        {
            if (AfterCheckFirstLaunch is null)
            {
                return;
            }

            await AfterCheckFirstLaunch.Invoke();
        }
    }
}
