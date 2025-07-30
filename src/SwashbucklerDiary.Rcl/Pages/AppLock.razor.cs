using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class AppLock
    {
        private bool appLockBiometric;

        private string? appLockNumberPassword;

        private string? appLockPatternPassword;

        private StringNumber? tabs;

        [Inject]
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            NavigateController.DisableNavigate = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await BiometricAuthenticateAsync();
            }
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            appLockBiometric = SettingService.Get(it => it.AppLockBiometric);
            appLockNumberPassword = SettingService.Get(it => it.AppLockNumberPassword);
            appLockPatternPassword = SettingService.Get(it => it.AppLockPatternPassword);
        }

        private bool HasAppLockExcludeBiometric
            => !string.IsNullOrEmpty(appLockNumberPassword)
            || !string.IsNullOrEmpty(appLockPatternPassword);

        private async Task BiometricAuthenticateAsync()
        {
            if (!appLockBiometric)
            {
                return;
            }

            // 假如开启指纹解锁，又在系统设置中移除指纹，将关闭指纹解锁，如果没有其他验证方式，直接通过验证
            bool isBiometricSupported = await PlatformIntegration.IsBiometricSupported();
            if (isBiometricSupported)
            {
                bool isSuccess = await PlatformIntegration.BiometricAuthenticateAsync();
                if (isSuccess)
                {
                    VerificationSuccessful();
                }
            }
            else
            {
                appLockBiometric = false;
                await SettingService.RemoveAsync(it => it.AppLockBiometric);
                if (HasAppLockExcludeBiometric)
                {
                    await AlertService.Info(I18n.T("System fingerprint unlocking has been turned off, please use another unlocking method"));
                }
                else
                {
                    VerificationSuccessful();
                }
            }
        }

        private void HandleNumberLockOnFinish(LockFinishArguments args)
        {
            args.IsFail = args.Value != appLockNumberPassword;
            if (!args.IsFail)
            {
                VerificationSuccessful();
            }
        }

        public void HandlePatternLockOnFinish(LockFinishArguments args)
        {
            args.IsFail = args.Value != appLockPatternPassword;
            if (!args.IsFail)
            {
                VerificationSuccessful();
            }
        }

        private void VerificationSuccessful()
        {
            NavigateController.DisableNavigate = false;

            var args = AppLifecycle.ActivationArguments;
            AppLifecycle.ActivationArguments = null;
            if (args is not null && args.Kind != AppActivationKind.Launch)
            {
                AppLifecycle.Activate(args);
            }
            else
            {
                To("");
            }
        }

        private void ExitApp()
        {
            AppLifecycle.QuitApp();
        }
    }
}