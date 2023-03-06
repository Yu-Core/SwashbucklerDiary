using SwashbucklerDiary.Components;
using SwashbucklerDiary.Extend;

namespace SwashbucklerDiary.Pages
{
    public partial class SettingPage : PageComponentBase
    {
        private bool Title;
        private bool Markdown;
        private bool Privacy;
        private bool ShowPPSet;
        private bool ShowPPInput;
        private string? PrivatePassword;

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await UpdatePrivatePassword();
            await base.OnInitializedAsync();
        }

        private async Task LoadSettings()
        {
            Title = await SettingsService.Get("Title", false);
            Markdown = await SettingsService.Get("Markdown", false);
            Privacy = await SettingsService.GetPrivacy();
        }

        private async Task TitleChange(bool value)
        {
            await SettingsService.Save("Title", value);
        }

        private async Task MarkdownChange(bool value)
        {
            await SettingsService.Save("Markdown", value);
        }

        private async Task PrivacyChange(bool value)
        {
            await SettingsService.Save("Privacy", value);
            if (!value)
            {
                await AlertService.Success(I18n.T("Setting.Safe.CamouflageSuccess"));
            }
        }

        private string? GetSafeName()
        {
            return Privacy ? I18n.T("Setting.Safe.Name") : I18n.T("Mine.Data");
        }

        private string? GetDisplayPrivacy()
        {
            return Privacy ? I18n.T("Setting.Safe.DisplayPrivacy") : I18n.T("Setting.Safe.Mask");
        }

        private async Task SetPassword(string value)
        {
            ShowPPSet = false;
            PrivatePassword = value;
            await SettingsService.Save("PrivatePassword", value.MD5Encrytp32());
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
            Privacy = true;
        }

        private async Task PrivacyClick()
        {
            await UpdatePrivatePassword();
            if (!string.IsNullOrEmpty(PrivatePassword) && !Privacy)
            {
                ShowPPInput = true;
                return;
            }

            Privacy = !Privacy;
        }

        private async Task UpdatePrivatePassword()
        {
            PrivatePassword = await SettingsService.Get("PrivatePassword", "");
        }

        private string? GetPrivatePasswordSetState()
            => string.IsNullOrEmpty(PrivatePassword) ? I18n.T("Setting.Safe.NotSetPassword") : null;
    }
}
