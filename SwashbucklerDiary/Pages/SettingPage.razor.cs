using BlazorComponent;
using Masa.Blazor;
using SwashbucklerDiary.Components;

namespace SwashbucklerDiary.Pages
{
    public partial class SettingPage : PageComponentBase
    {
        private bool Title;
        private bool Markdown;
        private bool Privacy;
        private bool PrivacyConfirm;
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
                await PopupService.ToastAsync(it =>
                {
                    it.Type = AlertTypes.Success;
                    it.Title = I18n.T("Setting.Safe.CamouflageSuccess");
                });
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
            await SettingsService.Save("PrivatePassword", value);
            await PopupService.ToastSuccessAsync("PrivatePasswordSetSuccess");
        }

        private async Task InputPassword(string value)
        {
            ShowPPInput = false;
            var password = await SettingsService.Get("PrivatePassword", "");
            if(password != value)
            {
                await PopupService.ToastErrorAsync(I18n.T("Setting.Safe.PasswordError"));
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
