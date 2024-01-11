using Microsoft.JSInterop;
using System.Reflection;
using System.Text;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class VersionTracking : IVersionTracking
    {
        const string versionsKey = "VersionTracking.Versions";
        const string buildsKey = "VersionTracking.Builds";

        readonly IJSRuntime _jSRuntime;

        string versionString = "1.0.0.0";

        string buildString = "0";

        Dictionary<string, List<string>> versionTrail = null!;

        string LastInstalledVersion => versionTrail[versionsKey]?.LastOrDefault() ?? string.Empty;

        string LastInstalledBuild => versionTrail[buildsKey]?.LastOrDefault() ?? string.Empty;

        public VersionTracking(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }

        public async Task Track(Assembly assembly)
        {
            if (versionTrail != null)
                return;

            InitVersion(assembly);
            await InitVersionTracking();
        }

        void InitVersion(Assembly assembly)
        {
            assembly ??= Assembly.GetExecutingAssembly();

            var assemblyFileVersionAttribute = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            if (assemblyFileVersionAttribute == null)
            {
                return;
            }

            var version = new Version(assemblyFileVersionAttribute.Version);
            versionString = version.ToString();
            buildString = version.Build.ToString();
        }

        /// <summary>
        /// Initialize VersionTracking module, load data and track current version
        /// </summary>
        /// <remarks>
        /// For internal use. Usually only called once in production code, but multiple times in unit tests
        /// </remarks>
        internal async Task InitVersionTracking()
        {
            bool containsVersionsKey = await ContainsKey(versionsKey);
            if (!containsVersionsKey)
            {
                IsFirstLaunchEver = true;
            }
            else
            {
                bool containsBuildsKey = await ContainsKey(buildsKey);
                IsFirstLaunchEver = !containsBuildsKey;
            }

            if (IsFirstLaunchEver)
            {
                versionTrail = new(StringComparer.Ordinal)
                {
                    { versionsKey, new List<string>() },
                    { buildsKey, new List<string>() }
                };
            }
            else
            {
                var versionsHistory = await ReadHistory(versionsKey);
                var buildsHistory = await ReadHistory(buildsKey);
                versionTrail = new(StringComparer.Ordinal)
                {
                    { versionsKey, versionsHistory.ToList() },
                    { buildsKey, buildsHistory.ToList() }
                };
            }

            IsFirstLaunchForCurrentVersion = !versionTrail[versionsKey].Contains(CurrentVersion) || CurrentVersion != LastInstalledVersion;
            if (IsFirstLaunchForCurrentVersion)
            {
                // Avoid duplicates and move current version to end of list if already present
                versionTrail[versionsKey].RemoveAll(v => v == CurrentVersion);
                versionTrail[versionsKey].Add(CurrentVersion);
            }

            IsFirstLaunchForCurrentBuild = !versionTrail[buildsKey].Contains(CurrentBuild) || CurrentBuild != LastInstalledBuild;
            if (IsFirstLaunchForCurrentBuild)
            {
                // Avoid duplicates and move current build to end of list if already present
                versionTrail[buildsKey].RemoveAll(b => b == CurrentBuild);
                versionTrail[buildsKey].Add(CurrentBuild);
            }

            if (IsFirstLaunchForCurrentVersion || IsFirstLaunchForCurrentBuild)
            {
                await WriteHistory(versionsKey, versionTrail[versionsKey]);
                await WriteHistory(buildsKey, versionTrail[buildsKey]);
            }
        }

        public bool IsFirstLaunchEver { get; private set; }

        public bool IsFirstLaunchForCurrentVersion { get; private set; }

        public bool IsFirstLaunchForCurrentBuild { get; private set; }

        public string CurrentVersion => versionString;

        public string CurrentBuild => buildString;

        public string? PreviousVersion => GetPrevious(versionsKey);

        public string? PreviousBuild => GetPrevious(buildsKey);

        public string? FirstInstalledVersion => versionTrail[versionsKey].FirstOrDefault();

        public string? FirstInstalledBuild => versionTrail[buildsKey].FirstOrDefault();

        public IReadOnlyList<string> VersionHistory => versionTrail[versionsKey].ToArray();

        public IReadOnlyList<string> BuildHistory => versionTrail[buildsKey].ToArray();

        public bool IsFirstLaunchForVersion(string version)
            => CurrentVersion == version && IsFirstLaunchForCurrentVersion;

        public bool IsFirstLaunchForBuild(string build)
            => CurrentBuild == build && IsFirstLaunchForCurrentBuild;

        public string GetStatus()
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("VersionTracking");
            sb.AppendLine($"  IsFirstLaunchEver:              {IsFirstLaunchEver}");
            sb.AppendLine($"  IsFirstLaunchForCurrentVersion: {IsFirstLaunchForCurrentVersion}");
            sb.AppendLine($"  IsFirstLaunchForCurrentBuild:   {IsFirstLaunchForCurrentBuild}");
            sb.AppendLine();
            sb.AppendLine($"  CurrentVersion:                 {CurrentVersion}");
            sb.AppendLine($"  PreviousVersion:                {PreviousVersion}");
            sb.AppendLine($"  FirstInstalledVersion:          {FirstInstalledVersion}");
            sb.AppendLine($"  VersionHistory:                 [{string.Join(", ", VersionHistory)}]");
            sb.AppendLine();
            sb.AppendLine($"  CurrentBuild:                   {CurrentBuild}");
            sb.AppendLine($"  PreviousBuild:                  {PreviousBuild}");
            sb.AppendLine($"  FirstInstalledBuild:            {FirstInstalledBuild}");
            sb.AppendLine($"  BuildHistory:                   [{string.Join(", ", BuildHistory)}]");
            return sb.ToString();
        }

        async ValueTask<string[]> ReadHistory(string key)
        {
            var data = await Get<string?>(key);
            return data?.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
        }

        ValueTask WriteHistory(string key, IEnumerable<string> history)
            => Set(key, string.Join("|", history));

        string? GetPrevious(string key)
        {
            var trail = versionTrail[key];
            return (trail.Count >= 2) ? trail[trail.Count - 2] : null;
        }

        ValueTask<bool> ContainsKey(string key)
        {
            return _jSRuntime.InvokeAsync<bool>("localStorage.hasOwnProperty", key);
        }

        ValueTask<T> Get<T>(string key)
        {
            return _jSRuntime.InvokeAsync<T>("localStorage.getItem", key);
        }

        ValueTask Set(string key, string data)
        {
            return _jSRuntime.InvokeVoidAsync("localStorage.setItem", key, data);
        }
    }
}
