using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public override AppPlatform CurrentPlatform { get; }
            = new BlazorWebAssemblyAppPlatform();
    }
}
