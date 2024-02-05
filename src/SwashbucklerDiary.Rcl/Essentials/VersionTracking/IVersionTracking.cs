namespace SwashbucklerDiary.Rcl.Essentials
{
    /// <summary>
    /// The VersionTracking API provides an easy way to track an app's version on a device.
    /// </summary>
    public interface IVersionTracking
    {
        /// <summary>
        /// Gets a value indicating whether this is the first time this app has ever been launched on this device.
        /// </summary>
        bool IsFirstLaunchEver { get; }

        /// <summary>
        /// Gets a value indicating if this is the first launch of the app for the current version number.
        /// </summary>
        bool IsFirstLaunchForCurrentVersion { get; }

        /// <summary>
        /// Gets a value indicating if this is the first launch of the app for the current build number.
        /// </summary>
        bool IsFirstLaunchForCurrentBuild { get; }

        /// <summary>
        /// Gets the current version number of the app.
        /// </summary>
        string CurrentVersion { get; }

        /// <summary>
        /// Gets the current build of the app.
        /// </summary>
        string CurrentBuild { get; }

        /// <summary>
        /// Gets the version number for the previously run version.
        /// </summary>
        string? PreviousVersion { get; }

        /// <summary>
        /// Gets the build number for the previously run version.
        /// </summary>
        string? PreviousBuild { get; }

        /// <summary>
        /// Gets the version number of the first version of the app that was installed on this device.
        /// </summary>
        string? FirstInstalledVersion { get; }

        /// <summary>
        /// Gets the build number of first version of the app that was installed on this device.
        /// </summary>
        string? FirstInstalledBuild { get; }

        /// <summary>
        /// Gets the collection of version numbers of the app that ran on this device.
        /// </summary>
        IReadOnlyList<string> VersionHistory { get; }

        /// <summary>
        /// Gets the collection of build numbers of the app that ran on this device.
        /// </summary>
        IReadOnlyList<string> BuildHistory { get; }

        /// <summary>
        /// Determines if this is the first launch of the app for a specified version number.
        /// </summary>
        /// <param name="version">The version number.</param>
        /// <returns><see langword="true"/> if this is the first launch of the app for the specified version number; otherwise <see langword="false"/>.</returns>
        bool IsFirstLaunchForVersion(string version);

        /// <summary>
        /// Determines if this is the first launch of the app for a specified build number.
        /// </summary>
        /// <param name="build">The build number.</param>
        /// <returns><see langword="true"/> if this is the first launch of the app for the specified build number; otherwise <see langword="false"/>.</returns>
        bool IsFirstLaunchForBuild(string build);
    }
}
