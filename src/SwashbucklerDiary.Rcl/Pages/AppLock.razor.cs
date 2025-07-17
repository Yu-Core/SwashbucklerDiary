using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class AppLock
    {
        private bool appLockBiometric;

        private string? appLockNumberPassword;

        private bool isBiometricSupported;

        [Inject]
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            isBiometricSupported = await PlatformIntegration.IsBiometricSupported();
            NavigateController.DisableNavigate = true;
            NavigateController.AddHistoryAction(AppLifecycle.QuitApp);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await BiometricAuthenticateAsync();
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            NavigateController.RemoveHistoryAction(AppLifecycle.QuitApp);
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            appLockBiometric = SettingService.Get(it => it.AppLockBiometric);
            appLockNumberPassword = SettingService.Get(it => it.AppLockNumberPassword);
        }

        private async Task BiometricAuthenticateAsync()
        {
            if (!appLockBiometric || !isBiometricSupported)
            {
                return;
            }

            bool isSuccess = await PlatformIntegration.BiometricAuthenticateAsync();
            if (isSuccess)
            {
                VerificationSuccessful();
            }
        }

        private void HandleNumberLockOnFinish(NumberLockFinishArguments args)
        {
            args.IsFail = args.Value != appLockNumberPassword;
            if (!args.IsFail)
            {
                VerificationSuccessful();
            }
        }

        private void VerificationSuccessful()
        {
            NavigateController.DisableNavigate = false;
            NavigateController.RemoveHistoryAction(AppLifecycle.QuitApp);

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
    }
}