using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using Theme = SwashbucklerDiary.Shared.Theme;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class PrivacyModePage : ImportantComponentBase
    {
        private bool _isRender = true;

        private bool privacyMode;

        private bool allowEnterPrivacyMode;

        private bool hidePrivacyModeEntrance;

        private string? privacyModeEntrancePassword;

        private bool showPrivacyModeEntrancePasswordDialog;

        private string? privacyModeFunctionSearchKey;

        private bool showPrivacyModeFunctionSearchKeyDialog;

        private bool showSetPrivacyDiary;

        private bool enablePrivacyModeDark;

        private bool showHidePrivacyModeEntranceConfirmDialog;

        [Inject]
        private BreakpointService BreakpointService { get; set; } = default!;

        [Inject]
        private IThemeService ThemeService { get; set; } = default!;

        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            BreakpointService.BreakpointChanged += HandleBreakpointChange;

            // Prevent entry through URL
            if (!allowEnterPrivacyMode)
            {
                _isRender = false;
                _ = NavigateToBack();
            }
            else
            {
                allowEnterPrivacyMode = false;
                SettingService.SetTemp(s => s.AllowEnterPrivacyMode, allowEnterPrivacyMode);
            }
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();
            privacyMode = SettingService.GetTemp(s => s.PrivacyMode);
            allowEnterPrivacyMode = SettingService.GetTemp(s => s.AllowEnterPrivacyMode);
            hidePrivacyModeEntrance = SettingService.Get(s => s.HidePrivacyModeEntrance);
            privacyModeEntrancePassword = SettingService.Get(s => s.PrivacyModeEntrancePassword);
            privacyModeFunctionSearchKey = SettingService.Get(s => s.PrivacyModeFunctionSearchKey, I18n.T("Privacy mode"));
            showSetPrivacyDiary = SettingService.Get(s => s.SetPrivacyDiary);
            enablePrivacyModeDark = SettingService.Get(s => s.PrivacyModeDark);
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            BreakpointService.BreakpointChanged -= HandleBreakpointChange;
        }

        private bool Desktop => BreakpointService.Breakpoint.SmAndUp;

        private bool Mobile => !Desktop;

        private string? SwitchPrivacyModeText => privacyMode ? "Exit privacy mode" : "Enter privacy mode";

        private string? PrivacyModeEntrancePasswordSetStateText
            => string.IsNullOrEmpty(privacyModeEntrancePassword) ? I18n.T("No password set") : I18n.T("Password has been set");

        private void SwitchPrivacyMode()
        {
            privacyMode = !privacyMode;
            SettingService.SetTemp(s => s.PrivacyMode, privacyMode);

            if (privacyMode)
            {
                if (enablePrivacyModeDark)
                {
                    ThemeService.SetTheme(Theme.Dark);
                }
            }
            else
            {
                int theme = SettingService.Get(s => s.Theme);
                ThemeService.SetTheme(theme);
            }

            To("");
        }

        private void HandleBreakpointChange(object? sender, MyBreakpointChangedEventArgs e)
        {
            if (!e.SmAndUpChanged)
            {
                return;
            }

            InvokeAsync(StateHasChanged);
        }

        private async Task SavePrivacyModeSearchKey(string value)
        {
            showPrivacyModeFunctionSearchKeyDialog = false;
            if (!string.IsNullOrWhiteSpace(value) && value != privacyModeFunctionSearchKey)
            {
                privacyModeFunctionSearchKey = value;
                await SettingService.SetAsync(s => s.PrivacyModeFunctionSearchKey, privacyModeFunctionSearchKey);
            }
        }

        private async Task SetPassword(string value)
        {
            showPrivacyModeEntrancePasswordDialog = false;
            privacyModeEntrancePassword = value;

            string hashedPassword = PasswordHasher.HashPasswordWithSalt(value, out string saltBase64);

            await SettingService.SetAsync(s => s.PrivacyModeEntrancePassword, hashedPassword);
            await SettingService.SetAsync(s => s.PrivacyModeEntrancePasswordSalt, saltBase64);
            await AlertService.SuccessAsync(I18n.T("Password set successfully"));
        }

        private async Task SwitchHidePrivacyModeEntrance()
        {
            if (!hidePrivacyModeEntrance)
            {
                showHidePrivacyModeEntranceConfirmDialog = true;
                return;
            }

            hidePrivacyModeEntrance = false;
            await SettingService.SetAsync(s => s.HidePrivacyModeEntrance, hidePrivacyModeEntrance);
        }

        private async Task ConfirmHidePrivacyModeEntrance()
        {
            showHidePrivacyModeEntranceConfirmDialog = false;
            hidePrivacyModeEntrance = true;
            await SettingService.SetAsync(s => s.HidePrivacyModeEntrance, hidePrivacyModeEntrance);
        }

        private async Task MoveOldPrivacyDiaries()
        {
            AlertService.StartLoading();
            try
            {
                var isSuccess = await DiaryService.MovePrivacyDiariesAsync();
                if (isSuccess)
                {
                    await AlertService.SuccessAsync(I18n.T("Successfully moved"));
                }
                else
                {
                    await AlertService.InfoAsync(I18n.T("No diary that needs to be moved"));
                }
            }
            finally
            {
                AlertService.StopLoading();
            }
        }

        private async Task ResetPassword()
        {
            showPrivacyModeEntrancePasswordDialog = false;
            privacyModeEntrancePassword = string.Empty;

            await SettingService.RemoveAsync(s => s.PrivacyModeEntrancePassword);
        }
    }
}