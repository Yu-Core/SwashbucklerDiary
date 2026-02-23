#nullable enable
using Microsoft.Maui.ApplicationModel;

namespace Microsoft.Maui.Storage
{
    // from https://github.com/dotnet/maui/blob/8423d5c68930aa6863b1e02e3eaf31c37db6d513/src/Essentials/src/FileSystem/FileSystemUtils.shared.cs
    // from https://github.com/dotnet/maui/blob/8423d5c68930aa6863b1e02e3eaf31c37db6d513/src/Essentials/src/FileSystem/FileSystemUtils.windows.cs
    static partial class FileSystemUtils
    {
        /// <summary>
        /// Normalizes the given file path for the current platform.
        /// </summary>
        /// <param name="filename">The file path to normalize.</param>
        /// <returns>
        /// The normalized version of the file path provided in <paramref name="filename"/>.
        /// Forward and backward slashes will be replaced by <see cref="Path.DirectorySeparatorChar"/>
        /// so that it is correct for the current platform.
        /// </returns>
        public static string NormalizePath(string filename) =>
            filename
                .Replace('\\', Path.DirectorySeparatorChar)
                .Replace('/', Path.DirectorySeparatorChar);

        public static string PlatformGetFullAppPackageFilePath(string filename)
        {
            if (filename is null)
                throw new ArgumentNullException(nameof(filename));

            filename = NormalizePath(filename);
            return Path.Combine(AppInfoUtils.PlatformGetFullAppPackageFilePath, filename);
        }
    }
}