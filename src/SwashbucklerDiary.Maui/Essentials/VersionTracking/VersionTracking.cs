namespace SwashbucklerDiary.Maui.Essentials
{
    public class VersionTracking : Rcl.Essentials.IVersionTracking
    {
        private static IVersionTracking VersionTrackingDefault => Microsoft.Maui.ApplicationModel.VersionTracking.Default;

        public bool IsFirstLaunchEver => VersionTrackingDefault.IsFirstLaunchEver;

        public bool IsFirstLaunchForCurrentVersion => VersionTrackingDefault.IsFirstLaunchForCurrentVersion;

        public bool IsFirstLaunchForCurrentBuild => VersionTrackingDefault.IsFirstLaunchForCurrentBuild;

        public string CurrentVersion => VersionTrackingDefault.CurrentVersion;

        public string CurrentBuild => VersionTrackingDefault.CurrentBuild;

        public string? PreviousVersion => VersionTrackingDefault.PreviousVersion;

        public string? PreviousBuild => VersionTrackingDefault.PreviousBuild;

        public string? FirstInstalledVersion => VersionTrackingDefault.FirstInstalledVersion;

        public string? FirstInstalledBuild => VersionTrackingDefault.FirstInstalledBuild;

        public IReadOnlyList<string> VersionHistory => VersionTrackingDefault.VersionHistory;

        public IReadOnlyList<string> BuildHistory => VersionTrackingDefault.BuildHistory;

        public bool IsFirstLaunchForBuild(string build)
            => VersionTrackingDefault.IsFirstLaunchForBuild(build);

        public bool IsFirstLaunchForVersion(string version)
            => VersionTrackingDefault.IsFirstLaunchForVersion(version);
    }
}
