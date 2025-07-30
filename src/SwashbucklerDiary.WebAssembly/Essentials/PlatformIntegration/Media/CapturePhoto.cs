namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public ValueTask<bool> IsCaptureSupported()
        {
            return ValueTask.FromResult(false);
        }

        public Task<string?> CapturePhotoAsync()
        {
            return Task.FromResult<string?>(null);
        }
    }
}
