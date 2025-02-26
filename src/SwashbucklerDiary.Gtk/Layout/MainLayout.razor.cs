using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Layout;

namespace SwashbucklerDiary.Gtk.Layout
{
    public partial class MainLayout : MainLayoutBase
    {
        private bool showUpdate;

        [Inject]
        private ILogger<MainLayout> Logger { get; set; } = default!;

        [Inject]
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            AppLifecycle.OnActivated += HandleActivated;
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
            AppLifecycle.OnActivated -= HandleActivated;
            VersionUpdataManager.AfterCheckFirstLaunch -= CheckForUpdates;
            base.OnDispose();
        }

#if DEBUG
        private void CheckForUpdates()
        {
        }
#else
        private async void CheckForUpdates()
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

        private void HandleActivated(ActivationArguments? args)
        {
            if (args is null || args.Data is null)
            {
                return;
            }

            switch (args.Kind)
            {
                case AppActivationKind.Scheme:
                    HandleScheme(args);
                    break;
                default:
                    break;
            }
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
