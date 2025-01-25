namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {
        static readonly string[] zipTypes = ["application/zip"];

        public Task<string?> PickZipFileAsync()
        {
            return PickFileAsync(zipTypes, ".zip");
        }
    }
}
