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
        private MasaBlazorHelper MasaBlazorHelper { get; set; } = default!;

        [Inject]
        private IThemeService ThemeService { get; set; } = default!;

        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            MasaBlazorHelper.BreakpointChanged += HandleBreakpointChange;

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
            privacyModeFunctionSearchKey = SettingService.Get(s => s.PrivacyModeFunctionSearchKey, I18n.T("PrivacyMode.Name"));
            showSetPrivacyDiary = SettingService.Get(s => s.SetPrivacyDiary);
            enablePrivacyModeDark = SettingService.Get(s => s.PrivacyModeDark);
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            MasaBlazorHelper.BreakpointChanged -= HandleBreakpointChange;
        }

        private bool Desktop => MasaBlazorHelper.Breakpoint.SmAndUp;

        private bool Mobile => !Desktop;

        private string? SwitchPrivacyModeText => privacyMode ? "PrivacyMode.QuitPrivacyMode" : "PrivacyMode.EnterPrivacyMode";

        private string? PrivacyModeEntrancePasswordSetStateText
            => string.IsNullOrEmpty(privacyModeEntrancePassword) ? I18n.T("PrivacyMode.No password set") : I18n.T("PrivacyMode.Password has been set");

        private async Task SwitchPrivacyMode()
        {
            privacyMode = !privacyMode;
            SettingService.SetTemp(s => s.PrivacyMode, privacyMode);

            if (privacyMode)
            {
                if (enablePrivacyModeDark)
                {
                    await ThemeService.SetThemeAsync(Theme.Dark);
                }
            }
            else
            {
                int theme = SettingService.Get(s => s.Theme);
                await ThemeService.SetThemeAsync((Theme)theme);
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

            string salt = nameof(Setting.PrivacyModeEntrancePassword);
            await SettingService.SetAsync(s => s.PrivacyModeEntrancePassword, (value + salt).MD5Encrytp32());
            await PopupServiceHelper.Success(I18n.T("PrivacyMode.PasswordSetSuccess"));
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
            await PopupServiceHelper.StartLoading();
            var isSuccess = await DiaryService.MovePrivacyDiariesAsync();
            await PopupServiceHelper.StopLoading();
            if (isSuccess)
            {
                await PopupServiceHelper.Success(I18n.T("PrivacyMode.Successfully moved"));
            }
            else
            {
                await PopupServiceHelper.Info(I18n.T("PrivacyMode.No diary that needs to be moved"));
            }
        }
    }
}