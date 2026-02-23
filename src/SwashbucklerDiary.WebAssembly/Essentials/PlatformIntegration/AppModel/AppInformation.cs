using System.Globalization;
using System.Reflection;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        static readonly Version version = Assembly.GetEntryAssembly()!.GetName().Version!;
        public static string VersionString => version?.ToString() ?? string.Empty;
        public static string BuildString => version.Revision.ToString(CultureInfo.InvariantCulture);

        public override string AppVersionString => VersionString;
    }
}
