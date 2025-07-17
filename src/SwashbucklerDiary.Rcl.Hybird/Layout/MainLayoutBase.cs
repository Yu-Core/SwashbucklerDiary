using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Hybird.Layout
{
    public class MainLayoutBase : Rcl.Layout.MainLayoutBase
    {
        protected bool showUpdate;

        [Inject]
        protected ILogger<MainLayoutBase> Logger { get; set; } = default!;

        [Inject]
        protected IAppLifecycle AppLifecycle { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await VersionUpdataManager.HandleVersionUpdate();
            await InitSettingsAsync();
        }

        protected override void DialogNotificationCore()
        {
            base.DialogNotificationCore();

            CheckForUpdates();
        }

#if DEBUG
        protected void CheckForUpdates()
        {
        }
#else
        protected async void CheckForUpdates()
        {
            bool notPrompt = SettingService.Get(it => it.UpdateNotPrompt);
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
