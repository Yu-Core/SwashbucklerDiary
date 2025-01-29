using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {
        const string zipFilefilterName = "Zip file";

        static readonly string[] zipPatterns = GetPatterns(PlatformIntegrationHelper.ZipFileExtensions);

        public Task<string?> PickZipFileAsync()
        {
            return PickFileAsync(zipFilefilterName, zipPatterns, PlatformIntegrationHelper.ZipFileExtensions);
        }
    }
}
