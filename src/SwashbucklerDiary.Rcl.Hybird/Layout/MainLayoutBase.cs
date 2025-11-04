using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Hybird.Layout
{
    public class MainLayoutBase : Rcl.Layout.MainLayoutBase
    {
        protected bool showUpdate;

        protected Release? lastRelease;

        [Inject]
        protected ILogger<MainLayoutBase> Logger { get; set; } = default!;

        [Inject]
        protected IAppLifecycle AppLifecycle { get; set; } = default!;

        protected override async Task DialogNotificationCoreAsync()
        {
            await base.DialogNotificationCoreAsync();

            await CheckForUpdates();
        }

#if DEBUG
        protected Task CheckForUpdates()
        {
            return Task.CompletedTask;
        }
#else
        protected async Task CheckForUpdates()
        {
            bool notPrompt = await SettingService.GetAsync(nameof(Setting.UpdateNotPrompt), false);
            if (notPrompt)
            {
                return;
            }

            string key = "LastAutoCheckForUpdatesTime";
            DateTime dateTime = await SettingService.GetAsync(key, DateTime.MinValue);
            if (dateTime != DateTime.MinValue && (DateTime.Now - dateTime).TotalHours < 24)
            {
                return;
            }

            await SettingService.SetAsync(key, DateTime.Now);

            try
            {
                lastRelease = await VersionUpdataManager.GetLastReleaseAsync();
                if (lastRelease is not null)
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

        protected override ActivationArguments CreateAppLockActivationArguments()
        {
            var relativePath = NavigationManager.GetBaseRelativePath();
            return new ActivationArguments()
            {
                Kind = AppActivationKind.Scheme,
                Data = $"{SchemeConstants.SwashbucklerDiary}://{relativePath}"
            };
        }

        protected override void HandleSchemeActivation(ActivationArguments args, bool replace)
        {
            string? uriString = args?.Data as string;
            if (NavigateController.CheckUrlScheme(uriString, out var path))
            {
                To(path.TrimStart('/'), replace: replace);
            }
        }
    }
}
