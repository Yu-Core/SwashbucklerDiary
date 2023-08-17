using BlazorComponent;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class AlertSettingPage : PageComponentBase
    {
        private int _timeout;
        private bool ShowTimeout;
        private Dictionary<string, string> Timeouts = new();

        protected override async Task OnInitializedAsync()
        {
            SetTimeouts();
            await LoadSettings();
            await base.OnInitializedAsync();
        }

        private StringNumber Timeout
        {
            get => (_timeout / 1000).ToString();
            set => _timeout = value.ToInt32() * 1000;
        }

        private async Task LoadSettings()
        {
            _timeout = await SettingsService.Get(SettingType.AlertTimeout);
        }

        private async Task UpdateAlertTimeout(int value)
        {
            _timeout = value;
            await SettingsService.Save(SettingType.AlertTimeout, value);
        }

        private void SetTimeouts()
        {
            for (int i = 0; i < 5; i++)
            {
                Timeouts.Add((i + 1).ToString(), string.Empty);
            }
        }
    }
}
