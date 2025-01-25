using System.Globalization;
using System.Reflection;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {
        static readonly Assembly _launchingAssembly = Assembly.GetEntryAssembly();

        public static string PackageName => _launchingAssembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? string.Empty;

        public static string PublisherName => _launchingAssembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? string.Empty;

        static readonly Version version = _launchingAssembly.GetName().Version!;
        public static string VersionString => version?.ToString() ?? string.Empty;
        public static string BuildString => version.Revision.ToString(CultureInfo.InvariantCulture);

        public string AppVersionString
            => VersionString;
        public Task ShowSettingsUI()
        {
            return Task.CompletedTask;
        }
    }
}
