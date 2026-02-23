using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class AppLock
    {
        private bool appLockBiometric;

        private string appLockNumberPassword = string.Empty;

        private string appLockPatternPassword = string.Empty;

        private string appLockNumberPasswordSalt = string.Empty;

        private string appLockPatternPasswordSalt = string.Empty;

        private StringNumber? tabs;

        [Inject]
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        [Inject]
        private RouteMatcher RouteMatcher { get; set; } = default!;

        [Inject]
        private IAppLockService AppLockService { get; set; } = default!;

        [SupplyParameterFromQuery]
        private bool IsLeave { get; set; }

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            NavigateController.DisableNavigate = true;
            AppLifecycle.OnResumed += HandleOnResumed;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender && !IsLeave)
            {
                await BiometricAuthenticateAsync();
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            AppLifecycle.OnResumed -= HandleOnResumed;
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            appLockBiometric = SettingService.Get(it => it.AppLockBiometric);
            appLockNumberPassword = SettingService.Get(it => it.AppLockNumberPassword);
            appLockPatternPassword = SettingService.Get(it => it.AppLockPatternPassword);
            appLockNumberPasswordSalt = SettingService.Get(it => it.AppLockNumberPasswordSalt);
            appLockPatternPasswordSalt = SettingService.Get(it => it.AppLockPatternPasswordSalt);
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
                    await VerificationSuccessful();
                }
            }
            else
            {
                appLockBiometric = false;
                await SettingService.RemoveAsync(it => it.AppLockBiometric);
                if (HasAppLockExcludeBiometric)
                {
                    await AlertService.InfoAsync(I18n.T("System fingerprint unlocking has been turned off, please use another unlocking method"));
                }
                else
                {
                    await VerificationSuccessful();
                }
            }
        }

        private async Task HandleNumberLockOnFinish(LockFinishArguments args)
        {
            args.IsFail = !PasswordHasher.VerifyPassword(args.Value ?? string.Empty, appLockNumberPassword, appLockNumberPasswordSalt); ;
            if (!args.IsFail)
            {
                await VerificationSuccessful();
            }
        }

        public async Task HandlePatternLockOnFinish(LockFinishArguments args)
        {
            args.IsFail = !PasswordHasher.VerifyPassword(args.Value ?? string.Empty, appLockPatternPassword, appLockPatternPasswordSalt);
            if (!args.IsFail)
            {
                await VerificationSuccessful();
            }
        }

        private async Task VerificationSuccessful()
        {
            NavigateController.DisableNavigate = false;

            await AppLockService.OnValidationSucceededAsync();

            var args = AppLifecycle.ActivationArguments;
            AppLifecycle.ActivationArguments = null;
            if (args is not null && args.Kind != AppActivationKind.Launch)
            {
                AppLifecycle.Activate(args);
                return;
            }

            if (ReturnUrl is not null && RouteMatcher.CheckRouter(NavigationManager.ToRoute(ReturnUrl)))
            {
                To(ReturnUrl, replace: true);
                return;
            }

            To("");
        }

        private void ExitApp()
        {
            AppLifecycle.QuitApp();
        }

        private void HandleOnResumed()
        {
            InvokeAsync(BiometricAuthenticateAsync);
        }
    }
}