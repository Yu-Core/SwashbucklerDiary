namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<bool> CheckCameraPermission()
        {
            return CheckPermission<Permissions.Camera>();
        }

        public Task<bool> TryCameraPermission()
        {
            return TryPermission<Permissions.Camera>();
        }
    }
}
