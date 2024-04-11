using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Layout;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Layout
{
    public partial class MainLayout : MainLayoutBase
    {
        private bool showUpdate;

        [Inject]
        private ILogger<MainLayout> Logger { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

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
            ThemeService.OnChanged -= ThemeChanged;
            VersionUpdataManager.AfterCheckFirstLaunch -= CheckForUpdates;
            base.OnDispose();
        }

        protected override async Task InitSettingsAsync()
        {
            await base.InitSettingsAsync();
            await InitThemeAsync();
        }

        private async Task InitThemeAsync()
        {
            ThemeService.OnChanged += ThemeChanged;
            var themeState = SettingService.Get<int>(Setting.Theme);
            await ThemeService.SetThemeAsync((Theme)themeState);
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
            if (dateTime != DateTime.MinValue && (DateTime.Now - dateTime).TotalHours < 2)
            {
                return;
            }

            await SettingService.Set(key, DateTime.Now);

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
    }
}
