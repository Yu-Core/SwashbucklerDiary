using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class AlertSettingPage : ImportantComponentBase
    {
        private int timeout;

        private bool showTimeout;

        private bool achievementsAlert;

        private Dictionary<string, int> timeoutItems = [];

        protected override void OnInitialized()
        {
            base.OnInitialized();

            InitTimeoutItems();
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            timeout = SettingService.Get(s => s.AlertTimeout);
            achievementsAlert = SettingService.Get(s => s.AchievementsAlert);
        }

        private string TimeoutText => timeoutItems.FirstOrDefault(it => it.Value == timeout).Key;

        private async Task UpdateSetting()
        {
            await SettingService.SetAsync(s => s.AlertTimeout, timeout);
        }

        private void InitTimeoutItems()
        {
            Dictionary<string, int> items = [];
            for (int i = 0; i < 5; i++)
            {
                items.Add($"{i + 1}s", (i + 1) * 1000);
            }

            timeoutItems = items;
        }
    }
}
