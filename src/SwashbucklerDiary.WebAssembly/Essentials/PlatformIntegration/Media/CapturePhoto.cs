namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public async ValueTask<bool> IsCaptureSupported()
        {
            var module = await Module;
            return module.Invoke<bool>("isCaptureSupported");
        }

        public async Task<string?> CapturePhotoAsync()
        {
            var module = await Module;
            return await module.InvokeAsync<string>("capturePhotoAsync", null);
        }
    }
}
