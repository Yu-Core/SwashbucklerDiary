using System.Globalization;
using System.Reflection;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        static readonly Version version = Assembly.GetEntryAssembly()!.GetName().Version!;
        public static string VersionString => version?.ToString() ?? string.Empty;
        public static string BuildString => version.Revision.ToString(CultureInfo.InvariantCulture);

        public string AppVersionString
            => VersionString;

        public Task ShowSettingsUI()
        {
            //TODO: Browser environment cannot open browser settings, But it can be done through browser plugins or tampermonkey scripts
            //return OpenUri("edge://settings/profiles", true);

            return Task.CompletedTask;
        }
    }
}
