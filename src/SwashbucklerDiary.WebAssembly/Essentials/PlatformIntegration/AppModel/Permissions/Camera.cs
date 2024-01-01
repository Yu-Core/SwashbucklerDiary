namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public async Task<bool> CheckCameraPermission()
        {
            var module = await Module;
            return await module.InvokeAsync<bool>("checkCameraPermission", null);
        }

        public async Task<bool> TryCameraPermission()
        {
            bool isCaptureSupported = await IsCaptureSupported();
            if (!isCaptureSupported)
            {
                await _alertService.Error(_i18n.T("User.NoCapture"));
                return false;
            }

            var module = await Module;
            return await module.InvokeAsync<bool>("tryCameraPermission", null);
        }
    }
}
