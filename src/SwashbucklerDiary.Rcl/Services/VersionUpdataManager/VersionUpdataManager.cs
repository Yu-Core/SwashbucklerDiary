using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Shared;
using System.Net.Http.Json;

namespace SwashbucklerDiary.Rcl.Services
{
    public abstract class VersionUpdataManager : IVersionUpdataManager
    {
        public event Action? AfterVersionUpdate;

        protected int updateCount;

        protected const string LatestVersionApiUrl = "https://api.github.com/repos/Yu-Core/SwashbucklerDiary/releases/latest";

        protected const string zh_CNLatestVersionApiUrl = "https://gitee.com/api/v5/repos/Yu-core/SwashbucklerDiary/releases/latest";

        protected readonly IDiaryService _diaryService;

        protected readonly IResourceService _resourceService;

        protected readonly ISettingService _settingService;

        protected readonly IMediaResourceManager _mediaResourceManager;

        protected readonly II18nService _i18n;

        protected readonly IVersionTracking _versionTracking;

        protected readonly IDiaryFileManager _diaryFileManager;

        protected readonly IStaticWebAssets _staticWebAssets;

        protected readonly IAppFileSystem _appFileSystem;

        protected readonly IAvatarService _avatarService;

        private readonly Lazy<HttpClient> _httpClient;

        private readonly SortedDictionary<Version, Func<Task>> _versionHandlers = [];

        public VersionUpdataManager(IDiaryService diaryService,
            IResourceService resourceService,
            ISettingService settingService,
            IMediaResourceManager mediaResourceManager,
            II18nService i18n,
            IVersionTracking versionTracking,
            IDiaryFileManager diaryFileManager,
            IStaticWebAssets staticWebAssets,
            IAppFileSystem appFileSystem,
            IAvatarService avatarService)
        {
            _diaryService = diaryService;
            _resourceService = resourceService;
            _settingService = settingService;
            _mediaResourceManager = mediaResourceManager;
            _i18n = i18n;
            _versionTracking = versionTracking;
            _diaryFileManager = diaryFileManager;
            _staticWebAssets = staticWebAssets;
            _appFileSystem = appFileSystem;
            _avatarService = avatarService;
            _httpClient = new(() =>
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)");
                return httpClient;
            });

            InitializeVersionHandlers();
        }

        public async Task HandleVersionUpdate()
        {
            foreach (var handler in _versionHandlers)
            {
                await HandleVersionUpdate(handler.Key.ToString(), handler.Value).ConfigureAwait(false);
            }

            var version = await _staticWebAssets.ReadJsonAsync<string>("docs/update-instruction/version.json").ConfigureAwait(false);
            await HandleVersionUpdate(version, HandleUpdateInstruction).ConfigureAwait(false);
            if (updateCount > 0)
            {
                AfterVersionUpdate?.Invoke();
            }
        }

        public async Task<Release?> GetLastReleaseAsync()
        {
            var url = _i18n.Culture.Name == "zh-CN"
                ? zh_CNLatestVersionApiUrl
                : LatestVersionApiUrl;
            var release = await HttpClient.GetFromJsonAsync<Release>(url).ConfigureAwait(false);
            if (release is null || release.Tag_Name is null)
            {
                throw new Exception("Tag format error");
            }

            var latestVersion = new Version(release.Tag_Name.TrimStart('v'));
            var currentVersion = new Version(_versionTracking.CurrentVersion);
            if (latestVersion > currentVersion)
            {
                return release;
            }

            return null;
        }

        public abstract Task ToUpdate();

        protected HttpClient HttpClient => _httpClient.Value;

        protected virtual void InitializeVersionHandlers()
        {
            AddVersionHandler("0.69.7", HandleVersionUpdate697);
            AddVersionHandler("0.80.9", HandleVersionUpdate809);
            AddVersionHandler("0.87.8", HandleVersionUpdate878);
            AddVersionHandler("1.01.5", HandleVersionUpdate1015);
            AddVersionHandler("1.05.5", HandleVersionUpdate1055);
            AddVersionHandler("1.13.2", HandleVersionUpdate1132);
            AddVersionHandler("1.29.1", HandleVersionUpdate1291);
        }

        protected void AddVersionHandler(string versionString, Func<Task> handler)
        {
            var version = Version.Parse(versionString);
            _versionHandlers[version] = handler;
        }

        protected async Task HandleVersionUpdate(string versionString, Func<Task> func)
        {
            string? previousVersionString = _versionTracking.PreviousVersion;
            if (string.IsNullOrEmpty(previousVersionString))
            {
                return;
            }

            if (new Version(previousVersionString) >= new Version(versionString))
            {
                return;
            }

            bool alreadyUpdated = await _settingService.GetAsync($"VersionUpdate_{versionString}", false).ConfigureAwait(false);
            if (alreadyUpdated)
            {
                return;
            }

            updateCount++;
            await func.Invoke();
            await _settingService.SetAsync($"VersionUpdate_{versionString}", true).ConfigureAwait(false);
        }

        protected virtual Task HandleVersionUpdate697()
        {
            return Task.CompletedTask;
        }

        private async Task HandleVersionUpdate809()
        {
            var uri = $"docs/vditor-tutorial/{_i18n.Culture}.md";
            var content = await _staticWebAssets.ReadContentAsync(uri).ConfigureAwait(false);
            var diary = new DiaryModel()
            {
                Content = content,
            };
            await _diaryService.AddAsync(diary).ConfigureAwait(false);
        }

        private async Task HandleUpdateInstruction()
        {
            string content = await _staticWebAssets.ReadI18nContentAsync("docs/update-instruction/{0}.md", _i18n.Culture).ConfigureAwait(false);

            var diary = new DiaryModel()
            {
                Content = content,
            };
            await _diaryService.AddAsync(diary).ConfigureAwait(false);
        }

        private async Task HandleVersionUpdate878()
        {
            string[] keys = ["PrivacyMode", "PrivatePassword"];
            await _settingService.RemoveAsync(keys).ConfigureAwait(false);
        }

        private async Task HandleVersionUpdate1015()
        {
            var oldKey = "DiaryCardDateFormat";
            var oldValue = await _settingService.GetAsync(oldKey, string.Empty).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(oldValue))
            {
                await _settingService.RemoveAsync(oldKey).ConfigureAwait(false);
                await _settingService.SetAsync("DiaryCardTimeFormat", oldValue).ConfigureAwait(false);
            }
        }

        private async Task HandleVersionUpdate1055()
        {
            await _diaryService.MovePrivacyDiariesAsync().ConfigureAwait(false);
        }

        private async Task HandleVersionUpdate1132()
        {
            var oldKey = "UserName";
            var oldValue = await _settingService.GetAsync(oldKey, string.Empty).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(oldValue))
            {
                await _settingService.RemoveAsync(oldKey).ConfigureAwait(false);
                await _settingService.SetAsync("NickName", oldValue).ConfigureAwait(false);
            }
        }

        protected virtual async Task HandleVersionUpdate1291()
        {
            Dictionary<string, string> directoryPaths = new()
            {
                ["Image"] = _mediaResourceManager.MediaResourceDirectoryPaths[MediaResource.Image],
                ["Audio"] = _mediaResourceManager.MediaResourceDirectoryPaths[MediaResource.Audio],
                ["Video"] = _mediaResourceManager.MediaResourceDirectoryPaths[MediaResource.Video],
                ["Avatar"] = _avatarService.AvatarDirectoryPath,
            };
            foreach (var item in directoryPaths)
            {
                var oldPath = Path.Combine(_appFileSystem.AppDataDirectory, item.Key.ToString());
                if (Directory.Exists(oldPath))
                {
                    await _appFileSystem.MoveFolderAsync(oldPath, item.Value).ConfigureAwait(false);
                }
            }
        }
    }
}
