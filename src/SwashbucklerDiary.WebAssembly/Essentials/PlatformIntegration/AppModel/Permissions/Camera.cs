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
            //capture只支持移动端，更合适的方法应该是自己写一个用于拍照的全屏弹窗

            var module = await Module;
            return await module.InvokeAsync<bool>("tryCameraPermission", null);
        }
    }
}
