using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Server.Essentials
{
    public partial class PlatformIntegration
    {
        public override AppPlatform CurrentPlatform
            => AppPlatform.BrowserServer;
    }
}