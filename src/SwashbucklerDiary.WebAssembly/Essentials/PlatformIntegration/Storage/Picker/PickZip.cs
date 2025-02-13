using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<string?> PickZipFileAsync()
            => PickFileAsync(PlatformIntegrationHelper.ZipMimeTypes, PlatformIntegrationHelper.ZipFileExtensions);
    }
}
