using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<string?> PickPhotoAsync()
            => PickFileAsync(PlatformIntegrationHelper.ImageMimeTypes, PlatformIntegrationHelper.ImageFileExtensions);

        public Task<IEnumerable<string>?> PickMultiplePhotoAsync()
            => PickFilesAsync(PlatformIntegrationHelper.ImageMimeTypes, PlatformIntegrationHelper.ImageFileExtensions);
    }
}
