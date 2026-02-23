using Microsoft.Maui.ApplicationModel;

namespace SwashbucklerDiary.Server.Essentials
{
    public partial class PlatformIntegration
    {
        public override string AppVersionString => AppInfo.VersionString;
    }
}
