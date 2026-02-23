#nullable enable
namespace Microsoft.Maui.ApplicationModel
{
    //from https://github.com/dotnet/maui/blob/8423d5c68930aa6863b1e02e3eaf31c37db6d513/src/Essentials/src/AppInfo/AppInfo.shared.cs
    /// <summary>
    /// Represents information about the application.
    /// </summary>
    public interface IAppInfo
    {
        /// <summary>
        /// Gets the application package name or identifier.
        /// </summary>
        /// <remarks>On Android and iOS, this is the application package name. On Windows, this is the application GUID.</remarks>
        string PackageName { get; }

        /// <summary>
        /// Gets the application name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the application version as a string representation.
        /// </summary>
        string VersionString { get; }

        /// <summary>
        /// Gets the application version as a <see cref="Version"/> object.
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Gets the application build number.
        /// </summary>
        string BuildString { get; }
    }

    /// <summary>
    /// Represents information about the application.
    /// </summary>
    public static class AppInfo
    {
        /// <summary>
        /// Gets the application package name or identifier.
        /// </summary>
        /// <remarks>On Android and iOS, this is the application package name. On Windows, this is the application GUID.</remarks>
        public static string PackageName => Current.PackageName;

        /// <summary>
        /// Gets the application name.
        /// </summary>
        public static string Name => Current.Name;

        /// <summary>
        /// Gets the application version as a string representation.
        /// </summary>
        public static string VersionString => Current.VersionString;

        /// <summary>
        /// Gets the application version as a <see cref="Version"/> object.
        /// </summary>
        public static Version Version => Current.Version;

        /// <summary>
        /// Gets the application build number.
        /// </summary>
        public static string BuildString => Current.BuildString;

        static IAppInfo? currentImplementation;

        /// <summary>
        /// Provides the default implementation for static usage of this API.
        /// </summary>
        public static IAppInfo Current =>
            currentImplementation ??= new AppInfoImplementation();

        internal static void SetCurrent(IAppInfo? implementation) =>
            currentImplementation = implementation;
    }

}