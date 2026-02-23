using Microsoft.Maui.ApplicationModel;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public class VersionTracking : Rcl.Essentials.IVersionTracking
    {
        private readonly IVersionTracking _versionTracking;

        public VersionTracking()
        {
            _versionTracking = Microsoft.Maui.ApplicationModel.VersionTracking.Default;
        }

        public bool IsFirstLaunchEver => _versionTracking.IsFirstLaunchEver;

        public bool IsFirstLaunchForCurrentVersion => _versionTracking.IsFirstLaunchForCurrentVersion;

        public bool IsFirstLaunchForCurrentBuild => _versionTracking.IsFirstLaunchForCurrentBuild;

        public string CurrentVersion => _versionTracking.CurrentVersion;

        public string CurrentBuild => _versionTracking.CurrentBuild;

        public string? PreviousVersion => _versionTracking.PreviousVersion;

        public string? PreviousBuild => _versionTracking.PreviousBuild;

        public string? FirstInstalledVersion => _versionTracking.FirstInstalledVersion;

        public string? FirstInstalledBuild => _versionTracking.FirstInstalledBuild;

        public IReadOnlyList<string> VersionHistory => _versionTracking.VersionHistory;

        public IReadOnlyList<string> BuildHistory => _versionTracking.BuildHistory;

        public bool IsFirstLaunchForBuild(string build)
            => _versionTracking.IsFirstLaunchForBuild(build);

        public bool IsFirstLaunchForVersion(string version)
            => _versionTracking.IsFirstLaunchForVersion(version);
    }
}
