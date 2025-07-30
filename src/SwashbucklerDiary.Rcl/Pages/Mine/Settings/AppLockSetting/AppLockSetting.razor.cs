using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class AppLockSetting
    {
        private string? appLockNumberPassword;

        private string? appLockPatternPassword;

        private bool appLockBiometric;

        private bool lockAppWhenLeave;

        private bool showNumberLock;

        private bool showPatternLock;

        private bool isBiometricSupported;

        [Inject]
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        [Inject]
        MasaBlazor MasaBlazor { get; set; } = default!;

        [CascadingParameter(Name = "MasaBlazorCascadingTheme")]
        public string? MasaBlazorCascadingTheme { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            AppLifecycle.OnResumed += UpdateIsBiometricSupported;
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            AppLifecycle.OnResumed -= UpdateIsBiometricSupported;
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await UpdateIsBiometricSupportedAsync();
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            appLockNumberPassword = SettingService.Get(it => it.AppLockNumberPassword);
            appLockBiometric = SettingService.Get(it => it.AppLockBiometric);
            appLockPatternPassword = SettingService.Get(it => it.AppLockPatternPassword);
            lockAppWhenLeave = SettingService.Get(it => it.LockAppWhenLeave);
        }

        private async Task SetAppLockNumberPasswordAsync(string value)
        {
            showNumberLock = false;

            if (string.IsNullOrEmpty(appLockNumberPassword))
            {
                appLockNumberPassword = value;
                await SettingService.SetAsync(it => it.AppLockNumberPassword, appLockNumberPassword);
            }
            else
            {
                appLockNumberPassword = null;
                await SettingService.RemoveAsync(it => it.AppLockNumberPassword);
            }
        }

        private async Task SetAppLockPatternPasswordAsync(string value)
        {
            showPatternLock = false;

            if (string.IsNullOrEmpty(appLockPatternPassword))
            {
                appLockPatternPassword = value;
                await SettingService.SetAsync(it => it.AppLockPatternPassword, appLockPatternPassword);
            }
            else
            {
                appLockPatternPassword = null;
                await SettingService.RemoveAsync(it => it.AppLockPatternPassword);
            }
        }

        private async Task SetAppLockBiometricAsync()
        {
            if (!isBiometricSupported)
            {
                return;
            }

            bool isSuccess = await PlatformIntegration.BiometricAuthenticateAsync();
            if (isSuccess)
            {
                appLockBiometric = !appLockBiometric;
                await SettingService.SetAsync(it => it.AppLockBiometric, appLockBiometric);
            }
        }

        private async void UpdateIsBiometricSupported()
        {
            await UpdateIsBiometricSupportedAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task UpdateIsBiometricSupportedAsync()
        {
            isBiometricSupported = await PlatformIntegration.IsBiometricSupported();
        }
    }
}