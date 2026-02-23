using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Web.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<string?> PickZipFileAsync()
            => PickFileAsync(PlatformIntegrationHelper.ZipMimeTypes, PlatformIntegrationHelper.ZipFileExtensions);
    }
}
