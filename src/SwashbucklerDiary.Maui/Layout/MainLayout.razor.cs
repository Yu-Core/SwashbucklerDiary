using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Maui.Extensions;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Layout;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Layout
{
    public partial class MainLayout : MainLayoutBase
    {
        private bool showUpdate;

        private bool isAndroidOrIOS;

        [Inject]
        private ILogger<MainLayout> Logger { get; set; } = default!;

        [Inject]
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        [Inject]
        private IPlatformIntegration PlatformIntegration { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            isAndroidOrIOS = PlatformIntegration.CurrentPlatform == AppDevicePlatform.Android || PlatformIntegration.CurrentPlatform == AppDevicePlatform.iOS;
            AppLifecycle.Activated += HandleActivated;
            VersionUpdataManager.AfterCheckFirstLaunch += CheckForUpdates;
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await VersionUpdataManager.HandleVersionUpdate();
            await InitSettingsAsync();
        }

        protected override void OnDispose()
        {
            AppLifecycle.Activated -= HandleActivated;
            VersionUpdataManager.AfterCheckFirstLaunch -= CheckForUpdates;
            base.OnDispose();
        }

#if DEBUG
        private Task CheckForUpdates() => Task.CompletedTask;
#else
        private async Task CheckForUpdates()
        {
            bool notPrompt = SettingService.Get<bool>(Setting.UpdateNotPrompt);
            if (notPrompt)
            {
                return;
            }

            string key = "LastAutoCheckForUpdatesTime";
            DateTime dateTime = await SettingService.Get(key, DateTime.MinValue);
            if (dateTime != DateTime.MinValue && (DateTime.Now - dateTime).TotalHours < 24)
            {
                return;
            }

            await SettingService.SetAsync(key, DateTime.Now);

            try
            {
                bool hasNewVersion = await VersionUpdataManager.CheckForUpdates();
                if (hasNewVersion)
                {
                    showUpdate = true;
                    await InvokeAsync(StateHasChanged);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "VersionUpdate check failed");
            }
        }
#endif

        private void HandleActivated(ActivationArguments? args)
        {
            if (args is null || args.Data is null)
            {
                return;
            }

            switch (args.Kind)
            {
                case LaunchActivationKind.Share:
                    HandleShare(args);
                    break;
                case LaunchActivationKind.Scheme:
                    HandleScheme(args);
                    break;
                default:
                    break;
            }
        }

        private void HandleShare(ActivationArguments args)
        {
            if (NavigationManager.GetBaseRelativePath() == "write")
            {
                return;
            }

            AppLifecycle.ActivationArguments = args;
            NavigationManager.NavigateTo("write");
        }

        private void HandleScheme(ActivationArguments args)
        {
            string? uriString = args?.Data as string;
            if (NavigateController.CheckUrlScheme(uriString, out var path))
            {
                NavigationManager.NavigateTo(path.TrimStart('/'));
            }
        }
    }
}
