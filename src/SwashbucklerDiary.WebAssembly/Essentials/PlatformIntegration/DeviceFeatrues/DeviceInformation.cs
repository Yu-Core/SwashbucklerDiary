using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public override AppPlatform CurrentPlatform
            => AppPlatform.BrowserWasm;
    }
}
