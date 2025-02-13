namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<bool> CheckCameraPermission()
        {
            return Task.FromResult(false);
        }

        public Task<bool> TryCameraPermission()
        {
            return Task.FromResult(false);
        }
    }
}
