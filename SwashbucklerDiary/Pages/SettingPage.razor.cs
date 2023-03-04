using BlazorComponent;
using Masa.Blazor;
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

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await base.OnInitializedAsync();
        }

        private async Task LoadSettings()
        {
            Title = await SettingsService.Get("Title",false);
            Markdown = await SettingsService.Get("Markdown", false);
            Privacy = await SettingsService.GetPrivacy();
        }

        private async Task TitleChange(bool value)
        {
            await SettingsService.Save("Title",value);
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
            await SettingsService.Save("PrivatePassword", value.MD5Encrytp32());
            await AlertService.Success(I18n.T("Setting.Safe.PrivatePasswordSetSuccess"));
        }

        private async Task InputPassword(string value)
        {
            ShowPPInput = false;
            if(string.IsNullOrWhiteSpace(value))
            {
                return;
            }
            var password = await SettingsService.Get("PrivatePassword", "");
            if(password != value.MD5Encrytp32())
            {
                await AlertService.Error(I18n.T("Setting.Safe.PasswordError"));
                return;
            }
            Privacy = true;
        }

        private async Task PrivacyClick()
        {
            var password = await SettingsService.Get("PrivatePassword", "");
            if (!string.IsNullOrEmpty(password) && !Privacy)
            {
                ShowPPInput = true;
                return;
            }

            Privacy = !Privacy;
        }
    }
}
