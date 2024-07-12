using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Maui.Essentials;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Layout;
using SwashbucklerDiary.Shared;
using Theme = SwashbucklerDiary.Shared.Theme;

namespace SwashbucklerDiary.Maui.Layout
{
    public partial class MainLayout : MainLayoutBase
    {
        private bool showUpdate;

        [Inject]
        private ILogger<MainLayout> Logger { get; set; } = default!;

        [Inject]
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        [Inject]
        private IPlatformIntegration PlatformIntegration { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            AppLifecycle.Activated += Activated;
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
            AppLifecycle.Activated -= Activated;
            VersionUpdataManager.AfterCheckFirstLaunch -= CheckForUpdates;
            base.OnDispose();
        }

        protected override async Task InitSettingsAsync()
        {
            await base.InitSettingsAsync();
            await InitThemeAsync();
        }

        protected override void ThemeChanged(Theme theme)
        {
            base.ThemeChanged(theme);

            TitleBarOrStatusBar.SetTitleBarOrStatusBar(theme);
        }

        private bool IsAndroidOrIOS
            => PlatformIntegration.CurrentPlatform == AppDevicePlatform.Android || PlatformIntegration.CurrentPlatform == AppDevicePlatform.iOS;

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
            if (dateTime != DateTime.MinValue && (DateTime.Now - dateTime).TotalHours < 24)
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

        private void Activated(ActivationArguments? args)
        {
            if (args is null || args.Data is null)
            {
                return;
            }

            if (args.Kind == LaunchActivationKind.Share)
            {
                if (NavigationManager.GetBaseRelativePath() == "write")
                {
                    return;
                }

                AppLifecycle.ActivationArguments = args;
                NavigationManager.NavigateTo("write");
            }
            else if (args.Kind == LaunchActivationKind.Scheme)
            {
                string? uriString = args?.Data as string;
                if (Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
                {
                    NavigationManager.NavigateTo(uri.AbsolutePath.TrimStart('/'));
                }
            }

        }
    }
}
