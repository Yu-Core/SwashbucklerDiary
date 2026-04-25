using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Server.Essentials
{
    public partial class PlatformIntegration
    {
        public override AppPlatform CurrentPlatform { get; }
            = new BlazorServerAppPlatform();
    }
}