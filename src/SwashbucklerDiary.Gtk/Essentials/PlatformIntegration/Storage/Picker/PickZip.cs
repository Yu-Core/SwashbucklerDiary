namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {
        const string zipFilefilterName = "Zip file";

        static readonly string[] zipFileExtensions = [".zip"];

        static readonly string[] zipPatterns = GetPatterns(zipFileExtensions);

        public Task<string?> PickZipFileAsync()
        {
            return PickFileAsync(zipFilefilterName, zipPatterns, zipFileExtensions);
        }
    }
}
