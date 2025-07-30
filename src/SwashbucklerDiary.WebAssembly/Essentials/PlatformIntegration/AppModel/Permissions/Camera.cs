namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<bool> CheckCameraPermission()
        {
            return Task.FromResult(false);
        }

        public Task<bool> TryCameraPermission()
        {
            //capture只支持移动端，更合适的方法应该是自己写一个用于拍照的全屏弹窗
            return Task.FromResult(false);
        }
    }
}
