using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class AppLockSetting
    {
        private string? appLockNumberPassword;

        private string? appLockPatternPassword;

        private string? appLockNumberPasswordSalt;

        private string? appLockPatternPasswordSalt;

        private bool appLockBiometric;

        private bool lockAppWhenLeave;

        private bool showNumberLock;

        private bool showPatternLock;

        private bool isBiometricSupported;

        [Inject]
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        [Inject]
        private IAppLockService AppLockService { get; set; } = default!;

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
            appLockNumberPasswordSalt = SettingService.Get(it => it.AppLockNumberPasswordSalt);
            appLockPatternPasswordSalt = SettingService.Get(it => it.AppLockPatternPasswordSalt);
            lockAppWhenLeave = SettingService.Get(it => it.LockAppWhenLeave);
        }

        private async Task SetAppLockNumberPasswordAsync(string password)
        {
            showNumberLock = false;

            if (string.IsNullOrEmpty(appLockNumberPassword) || string.IsNullOrEmpty(appLockNumberPasswordSalt))
            {
                // 设置密码
                appLockNumberPassword = PasswordHasher.HashPasswordWithSalt(password, out appLockNumberPasswordSalt);

                await SettingService.SetAsync(s => s.AppLockNumberPassword, appLockNumberPassword);
                await SettingService.SetAsync(s => s.AppLockNumberPasswordSalt, appLockNumberPasswordSalt);
            }
            else
            {
                appLockNumberPassword = string.Empty;
                await SettingService.RemoveAsync(it => it.AppLockNumberPassword);
            }

            AppLockService.OnLockChanged();
        }

        private bool ValidateNumberPassword(string password)
        {
            if (string.IsNullOrEmpty(appLockNumberPassword) || string.IsNullOrEmpty(appLockNumberPasswordSalt))
            {
                return false;
            }

            return PasswordHasher.VerifyPassword(password, appLockNumberPassword, appLockNumberPasswordSalt);
        }

        private async Task SetAppLockPatternPasswordAsync(string password)
        {
            showPatternLock = false;

            if (string.IsNullOrEmpty(appLockPatternPassword) || string.IsNullOrEmpty(appLockPatternPasswordSalt))
            {
                appLockPatternPassword = PasswordHasher.HashPasswordWithSalt(password, out appLockPatternPasswordSalt);

                await SettingService.SetAsync(s => s.AppLockPatternPassword, appLockPatternPassword);
                await SettingService.SetAsync(s => s.AppLockPatternPasswordSalt, appLockPatternPasswordSalt);
            }
            else
            {
                appLockPatternPassword = string.Empty;
                await SettingService.RemoveAsync(it => it.AppLockPatternPassword);
            }

            AppLockService.OnLockChanged();
        }

        private bool ValidatePatternPassword(string password)
        {
            if (string.IsNullOrEmpty(appLockPatternPassword) || string.IsNullOrEmpty(appLockPatternPasswordSalt))
            {
                return false;
            }

            return PasswordHasher.VerifyPassword(password, appLockPatternPassword, appLockPatternPasswordSalt);
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
                AppLockService.OnLockChanged();
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