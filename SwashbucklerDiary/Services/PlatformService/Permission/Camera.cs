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
            if (!IsCaptureSupported())
            {
                await AlertService.Error(I18n.T("User.NoCapture"));
                return false;
            }

            return await TryPermission<Permissions.Camera>("Permission.OpenCamera");
        }
    }
}
