namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        public async Task<bool> CheckCameraPermission()
        {
            return await CheckPermission<Permissions.Camera>();
        }

        public async Task<bool> TryCameraPermission()
        {
            return await TryPermission<Permissions.Camera>();
        }
    }
}
