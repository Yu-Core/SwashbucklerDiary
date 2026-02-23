#nullable enable
namespace Microsoft.Maui.Storage
{
    // from https://github.com/dotnet/maui/blob/8423d5c68930aa6863b1e02e3eaf31c37db6d513/src/Essentials/src/FileSystem/FileSystem.shared.cs
    /// <summary>
    /// Provides an easy way to access the locations for device folders.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Gets the location where temporary data can be stored.
        /// </summary>
        /// <remarks>This location usually is not visible to the user, is not backed up, and may be cleared by the operating system at any time.</remarks>
        string CacheDirectory { get; }

        /// <summary>
        /// Gets the location where app data can be stored.
        /// </summary>
        /// <remarks>This location usually is not visible to the user, and is backed up.</remarks>
        string AppDataDirectory { get; }

        /// <summary>
        /// Opens a stream to a file contained within the app package.
        /// </summary>
        /// <param name="filename">The name of the file (excluding the path) to load from the app package.</param>
        /// <returns>A <see cref="Stream"/> containing the (read-only) file data.</returns>
        Task<Stream> OpenAppPackageFileAsync(string filename);

        /// <summary>
        /// Determines whether or not a file exists in the app package.
        /// </summary>
        /// <param name="filename">The name of the file (excluding the path) to load from the app package.</param>
        /// <returns><see langword="true"/> when the specified file exists in the app package, otherwise <see langword="false"/>.</returns>
        Task<bool> AppPackageFileExistsAsync(string filename);
    }

    /// <summary>
    /// Provides an easy way to access the locations for device folders.
    /// </summary>
    public static class FileSystem
    {
        /// <summary>
        /// Gets the location where temporary data can be stored.
        /// </summary>
        /// <remarks>This location usually is not visible to the user, is not backed up, and may be cleared by the operating system at any time.</remarks>
        public static string CacheDirectory
            => Current.CacheDirectory;

        /// <summary>
        /// Gets the location where app data can be stored.
        /// </summary>
        /// <remarks>This location usually is not visible to the user, and is backed up.</remarks>
        public static string AppDataDirectory
            => Current.AppDataDirectory;

        /// <summary>
        /// Opens a stream to a file contained within the app package.
        /// </summary>
        /// <param name="filename">The name of the file (excluding the path) to load from the app package.</param>
        /// <returns>A <see cref="Stream"/> containing the (read-only) file data.</returns>
        public static Task<Stream> OpenAppPackageFileAsync(string filename)
            => Current.OpenAppPackageFileAsync(filename);

        /// <summary>
        /// Determines whether or not a file exists in the app package.
        /// </summary>
        /// <param name="filename">The path of the file (relative to the app package) to check the existence of.</param>
        /// <returns><see langword="true"/> when the specified file exists in the app package, otherwise <see langword="false"/>.</returns>
        public static Task<bool> AppPackageFileExistsAsync(string filename)
            => Current.AppPackageFileExistsAsync(filename);

        static IFileSystem? currentImplementation;

        /// <summary>
        /// Provides the default implementation for static usage of this API.
        /// </summary>
        public static IFileSystem Current =>
            currentImplementation ??= new FileSystemImplementation();

        internal static void SetCurrent(IFileSystem? implementation) =>
            currentImplementation = implementation;
    }

    /// <summary>
    /// Concrete implementation of the <see cref="IFileSystem"/> APIs.
    /// </summary>
    public partial class FileSystemImplementation
    {
        /// <inheritdoc cref="IFileSystem.CacheDirectory"/>
        public string CacheDirectory
            => PlatformCacheDirectory;

        /// <inheritdoc cref="IFileSystem.AppDataDirectory"/>
        public string AppDataDirectory
            => PlatformAppDataDirectory;

        /// <inheritdoc cref="IFileSystem.OpenAppPackageFileAsync(string)"/>
        public Task<Stream> OpenAppPackageFileAsync(string filename)
            => PlatformOpenAppPackageFileAsync(filename);

        /// <inheritdoc cref="IFileSystem.AppPackageFileExistsAsync(string)"/>
        public Task<bool> AppPackageFileExistsAsync(string filename)
            => PlatformAppPackageFileExistsAsync(filename);
    }
}