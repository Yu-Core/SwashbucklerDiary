using System.Globalization;
using System.Reflection;

namespace Microsoft.Maui.ApplicationModel
{
    // from https://github.com/dotnet/maui/blob/8423d5c68930aa6863b1e02e3eaf31c37db6d513/src/Essentials/src/AppInfo/AppInfo.windows.cs
    class AppInfoImplementation : IAppInfo
    {
        static readonly Assembly _launchingAssembly = Assembly.GetEntryAssembly();

        public string PackageName =>
            _launchingAssembly.GetAppInfoValue("PackageName") ?? _launchingAssembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? string.Empty;

        public static string PublisherName =>
            _launchingAssembly.GetAppInfoValue("PublisherName") ?? _launchingAssembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? string.Empty;

        public string Name =>
            _launchingAssembly.GetAppInfoValue("Name") ?? _launchingAssembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? string.Empty;

        public Version Version =>
            _launchingAssembly.GetAppInfoVersionValue("Version") ?? _launchingAssembly.GetName().Version;

        public string VersionString => Version.ToString();

        public string BuildString => Version.Revision.ToString(CultureInfo.InvariantCulture);
    }

    static class AppInfoUtils
    {
        static readonly Lazy<string> platformGetFullAppPackageFilePath = new Lazy<string>(() =>
        {
            return AppContext.BaseDirectory;
        });

        /// <summary>
        /// Gets full application path.
        /// </summary>
        public static string PlatformGetFullAppPackageFilePath => platformGetFullAppPackageFilePath.Value;

        /// <summary>
        /// Gets the version information for this app.
        /// </summary>
        /// <param name="assembly">The assembly to retrieve the version information for.</param>
        /// <param name="name">The key that is used to retrieve the version information from the metadata.</param>
        /// <returns><see langword="null"/> if <paramref name="name"/> is <see langword="null"/> or empty, or no version information could be found with the value of <paramref name="name"/>.</returns>
        public static Version GetAppInfoVersionValue(this Assembly assembly, string name)
        {
            if (assembly.GetAppInfoValue(name) is string value && !string.IsNullOrEmpty(value))
                return Version.Parse(value);

            return null;
        }

        /// <summary>
        /// Gets the app info from this apps' metadata.
        /// </summary>
        /// <param name="assembly">The assembly to retrieve the app info for.</param>
        /// <param name="name">The key of the metadata to be retrieved (e.g. PackageName, PublisherName or Name).</param>
        /// <returns>The value that corresponds to the given key in <paramref name="name"/>.</returns>
        public static string GetAppInfoValue(this Assembly assembly, string name) =>
            assembly.GetMetadataAttributeValue("Microsoft.Maui.ApplicationModel.AppInfo." + name);

        /// <summary>
        /// Gets the value for a given key from the assembly metadata.
        /// </summary>
        /// <param name="assembly">The assembly to retrieve the information for.</param>
        /// <param name="key">The key of the metadata to be retrieved (e.g. PackageName, PublisherName or Name).</param>
        /// <returns>The value that corresponds to the given key in <paramref name="key"/>.</returns>
        public static string GetMetadataAttributeValue(this Assembly assembly, string key)
        {
            foreach (var attr in assembly.GetCustomAttributes<AssemblyMetadataAttribute>())
            {
                if (attr.Key == key)
                    return attr.Value;
            }

            return null;
        }
    }
}