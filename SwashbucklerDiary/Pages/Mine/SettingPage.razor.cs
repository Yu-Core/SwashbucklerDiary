using SwashbucklerDiary.Components;
using SwashbucklerDiary.Extend;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class SettingPage : PageComponentBase
    {
        private bool Title;
        private bool Markdown;
        private bool Privacy;
        private bool WelcomText;
        private string? PrivatePassword;
        private bool ShowPPSet;
        private bool ShowPPInput;

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await UpdatePrivatePassword();
            await base.OnInitializedAsync();
        }

        private string? MSwitchTrackColor(bool value)
        {
            return value && Light ? "black" : null;
        }

        private async Task LoadSettings()
        {
            Title = await SettingsService.Get(SettingType.Title);
            Markdown = await SettingsService.Get(SettingType.Markdown);
            Privacy = await SettingsService.Get(SettingType.PrivacyMode);
            WelcomText = await SettingsService.Get(SettingType.WelcomeText);
        }

        private async Task TitleChange(bool value)
        {
            await SettingsService.Save(SettingType.Title, value);
        }

        private async Task MarkdownChange(bool value)
        {
            await SettingsService.Save(SettingType.Markdown, value);
        }

        private async Task PrivacyChange(bool value)
        {
            Privacy = value;
            await SettingsService.Save(SettingType.PrivacyMode, value);
            if (!value)
            {
                await AlertService.Success(I18n.T("Setting.Safe.CamouflageSuccess"));
            }
        }

        private async Task WelcomTextChange(bool value)
        {
            await SettingsService.Save(SettingType.WelcomeText, value);
        }

        private string? GetDisplayPrivacy()
        {
            return Privacy ? I18n.T("Setting.Safe.DisplayPrivacy") : I18n.T("Setting.Safe.Mask");
        }

        private async Task SetPassword(string value)
        {
            ShowPPSet = false;
            PrivatePassword = value;
            await SettingsService.Save(SettingType.PrivatePassword, value.MD5Encrytp32());
            await AlertService.Success(I18n.T("Setting.Safe.PrivatePasswordSetSuccess"));
        }

        private async Task InputPassword(string value)
        {
            ShowPPInput = false;
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }
            await UpdatePrivatePassword();
            if (PrivatePassword != value.MD5Encrytp32())
            {
                await AlertService.Error(I18n.T("Setting.Safe.PasswordError"));
                return;
            }
            await PrivacyChange(true);
        }

        private async Task PrivacyClick()
        {
            await UpdatePrivatePassword();
            if (!string.IsNullOrEmpty(PrivatePassword) && !Privacy)
            {
                ShowPPInput = true;
                return;
            }

            await PrivacyChange(!Privacy);
        }

        private async Task UpdatePrivatePassword()
        {
            PrivatePassword = await SettingsService.Get(SettingType.PrivatePassword);
        }

        private string? GetPrivatePasswordSetState()
            => string.IsNullOrEmpty(PrivatePassword) ? I18n.T("Setting.Safe.NotSetPassword") : null;
    }
}
