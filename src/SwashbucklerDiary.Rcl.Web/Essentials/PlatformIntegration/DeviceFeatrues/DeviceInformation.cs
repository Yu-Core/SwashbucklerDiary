using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Web.Essentials
{
    public partial class PlatformIntegration
    {
        public abstract AppPlatform CurrentPlatform { get; }

        public string DeviceName => string.Empty;
    }
}
