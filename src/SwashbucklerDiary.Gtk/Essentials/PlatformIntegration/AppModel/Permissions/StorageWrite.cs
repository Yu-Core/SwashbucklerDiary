namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<bool> CheckStorageWritePermission()
        {
            return Task.FromResult(true);
        }

        public Task<bool> TryStorageWritePermission()
        {
            return Task.FromResult(true);
        }
    }
}
