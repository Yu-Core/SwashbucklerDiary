namespace SwashbucklerDiary.Rcl.Web.Essentials
{
    public partial class PlatformIntegration
    {
        public ValueTask<bool> IsBiometricSupported()
        {
            return _jsModule.IsBiometricSupported();
        }

        public async Task<bool> BiometricAuthenticateAsync()
        {
            return await _jsModule.BiometricAuthenticateAsync().ConfigureAwait(false);
        }
    }
}
