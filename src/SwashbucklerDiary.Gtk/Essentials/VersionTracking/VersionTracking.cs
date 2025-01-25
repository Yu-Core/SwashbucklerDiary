using System.Text;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public class VersionTracking : Rcl.Essentials.IVersionTracking
    {
        const string versionsKey = "VersionTracking.Versions";
        const string buildsKey = "VersionTracking.Builds";

        readonly UnpackagedPreferencesImplementation preferences;

        Dictionary<string, List<string>> versionTrail = null!;

        string LastInstalledVersion => versionTrail[versionsKey]?.LastOrDefault() ?? string.Empty;

        string LastInstalledBuild => versionTrail[buildsKey]?.LastOrDefault() ?? string.Empty;

        public VersionTracking(UnpackagedPreferencesImplementation preferences)
        {
            this.preferences = preferences;

            Track();
        }

        public void Track()
        {
            if (versionTrail != null)
                return;

            InitVersionTracking();
        }

        /// <summary>
        /// Initialize VersionTracking module, load data and track current version
        /// </summary>
        /// <remarks>
        /// For internal use. Usually only called once in production code, but multiple times in unit tests
        /// </remarks>
        internal void InitVersionTracking()
        {
            IsFirstLaunchEver = !preferences.ContainsKey(versionsKey) || !preferences.ContainsKey(buildsKey);
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
                versionTrail = new(StringComparer.Ordinal)
                {
                    { versionsKey, ReadHistory(versionsKey).ToList() },
                    { buildsKey, ReadHistory(buildsKey).ToList() }
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
                WriteHistory(versionsKey, versionTrail[versionsKey]);
                WriteHistory(buildsKey, versionTrail[buildsKey]);
            }
        }

        public bool IsFirstLaunchEver { get; private set; }

        public bool IsFirstLaunchForCurrentVersion { get; private set; }

        public bool IsFirstLaunchForCurrentBuild { get; private set; }

        public string CurrentVersion => PlatformIntegration.VersionString;

        public string CurrentBuild => PlatformIntegration.BuildString;

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

        string[] ReadHistory(string key)
            => preferences.Get<string?>(key, null)?.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

        void WriteHistory(string key, IEnumerable<string> history)
            => preferences.Set(key, string.Join("|", history));

        string? GetPrevious(string key)
        {
            var trail = versionTrail[key];
            return (trail.Count >= 2) ? trail[trail.Count - 2] : null;
        }
    }
}
