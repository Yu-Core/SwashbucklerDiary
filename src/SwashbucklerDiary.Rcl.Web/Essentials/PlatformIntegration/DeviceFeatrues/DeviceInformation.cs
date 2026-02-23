using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Web.Essentials
{
    public partial class PlatformIntegration
    {
        public abstract AppPlatform CurrentPlatform { get; }

        public string DeviceName => string.Empty;
    }
}
