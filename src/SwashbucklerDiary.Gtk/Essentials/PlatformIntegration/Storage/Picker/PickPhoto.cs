using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {
        const string imageFilefilterName = "Image file";

        const string imageFilesfilterName = "Image files";

        static readonly string[] imagePatterns = GetPatterns(PlatformIntegrationHelper.ImageFileExtensions);

        public Task<string?> PickPhotoAsync()
            => PickFileAsync(imageFilefilterName, imagePatterns, PlatformIntegrationHelper.ImageFileExtensions);

        public Task<IEnumerable<string>?> PickMultiplePhotoAsync()
            => PickMultipleFileAsync(imageFilesfilterName, imagePatterns, PlatformIntegrationHelper.ImageFileExtensions);
    }
}
