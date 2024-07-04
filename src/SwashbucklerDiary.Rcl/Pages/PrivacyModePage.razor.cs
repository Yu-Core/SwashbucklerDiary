using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;
using Theme = SwashbucklerDiary.Shared.Theme;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class PrivacyModePage : ImportantComponentBase
    {
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
        private MasaBlazor MasaBlazor { get; set; } = default!;

        [Inject]
        private IThemeService ThemeService { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            MasaBlazor.BreakpointChanged += InvokeStateHasChanged;

            // Prevent entry through URL
            if (!allowEnterPrivacyMode)
            {
                _ = NavigateToBack();
            }
            else
            {
                allowEnterPrivacyMode = false;
                SettingService.SetTemp<bool>(TempSetting.AllowEnterPrivacyMode, allowEnterPrivacyMode);
            }
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();
            privacyMode = SettingService.GetTemp<bool>(TempSetting.PrivacyMode);
            allowEnterPrivacyMode = SettingService.GetTemp<bool>(TempSetting.AllowEnterPrivacyMode);
            hidePrivacyModeEntrance = SettingService.Get<bool>(Setting.HidePrivacyModeEntrance);
            privacyModeEntrancePassword = SettingService.Get<string>(Setting.PrivacyModeEntrancePassword);
            privacyModeFunctionSearchKey = SettingService.Get<string>(Setting.PrivacyModeFunctionSearchKey, I18n.T("PrivacyMode.Name"));
            showSetPrivacyDiary = SettingService.Get<bool>(Setting.SetPrivacyDiary);
            enablePrivacyModeDark = SettingService.Get<bool>(Setting.PrivacyModeDark);
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            MasaBlazor.BreakpointChanged -= InvokeStateHasChanged;
        }

        private bool Desktop => MasaBlazor.Breakpoint.SmAndUp;

        private bool Mobile => !Desktop;

        private string? SwitchPrivacyModeText => privacyMode ? "PrivacyMode.QuitPrivacyMode" : "PrivacyMode.EnterPrivacyMode";

        private string? PrivacyModeEntrancePasswordSetStateText
            => string.IsNullOrEmpty(privacyModeEntrancePassword) ? I18n.T("PrivacyMode.No password set") : I18n.T("PrivacyMode.Password has been set");

        private async Task SwitchPrivacyMode()
        {
            privacyMode = !privacyMode;
            SettingService.SetTemp<bool>(TempSetting.PrivacyMode, privacyMode);

            if (privacyMode)
            {
                if (enablePrivacyModeDark)
                {
                    await ThemeService.SetThemeAsync(Theme.Dark);
                }
            }
            else
            {
                int theme = SettingService.Get<int>(Setting.Theme);
                await ThemeService.SetThemeAsync((Theme)theme);
            }

            To("");
        }

        private void InvokeStateHasChanged(object? sender, BreakpointChangedEventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }

        private async Task SavePrivacyModeSearchKey(string value)
        {
            showPrivacyModeFunctionSearchKeyDialog = false;
            if (!string.IsNullOrWhiteSpace(value) && value != privacyModeFunctionSearchKey)
            {
                privacyModeFunctionSearchKey = value;
                await SettingService.Set(Setting.PrivacyModeFunctionSearchKey, privacyModeFunctionSearchKey);
            }
        }

        private async Task SetPassword(string value)
        {
            showPrivacyModeEntrancePasswordDialog = false;
            privacyModeEntrancePassword = value;

            string salt = Setting.PrivacyModeEntrancePassword.ToString();
            await SettingService.Set(Setting.PrivacyModeEntrancePassword, (value + salt).MD5Encrytp32());
            await AlertService.Success(I18n.T("PrivacyMode.PasswordSetSuccess"));
        }

        private async Task SwitchHidePrivacyModeEntrance()
        {
            if (!hidePrivacyModeEntrance)
            {
                showHidePrivacyModeEntranceConfirmDialog = true;
                return;
            }

            hidePrivacyModeEntrance = false;
            await SettingService.Set(Setting.HidePrivacyModeEntrance, hidePrivacyModeEntrance);
        }

        private async Task ConfirmHidePrivacyModeEntrance()
        {
            showHidePrivacyModeEntranceConfirmDialog = false;
            hidePrivacyModeEntrance = true;
            await SettingService.Set(Setting.HidePrivacyModeEntrance, hidePrivacyModeEntrance);
        }
    }
}