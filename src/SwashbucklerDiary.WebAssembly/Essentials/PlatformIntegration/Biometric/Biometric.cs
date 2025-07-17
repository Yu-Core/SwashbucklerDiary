namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public ValueTask<bool> IsBiometricSupported()
        {
            return ValueTask.FromResult(false);
        }

        public Task<bool> BiometricAuthenticateAsync()
        {
            return Task.FromResult(false);
        }
    }
}
