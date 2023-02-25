using SwashbucklerDiary.Components;

namespace SwashbucklerDiary.Pages
{
    public partial class SettingPage : PageComponentBase
    {
        private bool Title;
        private bool Markdown;
        private bool Privacy;

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

        private async Task TitleChanged(bool value)
        {
            Title = value;
            await SettingsService.Save("Title",value);
        }

        private async Task MarkdownChanged(bool value)
        {
            Markdown = value;
            await SettingsService.Save("Markdown", value);
        }

        private async Task PrivacyChanged(bool value)
        {
            Privacy = value;
            await SettingsService.Save("Privacy", value);
        }

        private string? GetSafeName()
        {
            return Privacy ? I18n.T("Setting.Safe.Name") : I18n.T("Mine.Data");
        }

        private string? GetDisplayPrivacy()
        {
            return Privacy ? I18n.T("Setting.Safe.DisplayPrivacy") : I18n.T("Setting.Safe.Mask");
        }
    }
}
