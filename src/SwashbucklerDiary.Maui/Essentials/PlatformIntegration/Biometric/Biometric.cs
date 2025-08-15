using Plugin.Maui.Biometric;

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        public async ValueTask<bool> IsBiometricSupported()
        {
            var biometric = BiometricAuthenticationService.Default;
            var enrolledTypes = await biometric.GetEnrolledBiometricTypesAsync().ConfigureAwait(false);
            var result = await biometric.GetAuthenticationStatusAsync().ConfigureAwait(false);

            bool isSupported = BiometricAuthenticationService.Default.IsPlatformSupported &&
                !enrolledTypes.Contains(BiometricType.None) &&
                result == BiometricHwStatus.Success;

            return isSupported;
        }

        public async Task<bool> BiometricAuthenticateAsync()
        {
            var request = new AuthenticationRequest()
            {
                Title = _i18n.T("Authentication"),
                NegativeText = _i18n.T("Cancel"),
                AllowPasswordAuth = false
            };

#if WINDOWS
            request.Description = _i18n.T("Authentication");
#endif

            var response = await BiometricAuthenticationService.Default.AuthenticateAsync(request, CancellationToken.None).ConfigureAwait(false);

            return response.Status == BiometricResponseStatus.Success;
        }
    }
}
