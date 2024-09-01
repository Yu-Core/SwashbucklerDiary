using Masa.Blazor;
using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class AlertSettingPage : ImportantComponentBase
    {
        private int _timeout;

        private bool showTimeout;

        private bool achievementsAlert;

        private readonly Dictionary<string, string> timeoutItems = [];

        protected override void OnInitialized()
        {
            base.OnInitialized();

            InitTimeoutItems();
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            _timeout = SettingService.Get(s => s.AlertTimeout);
            achievementsAlert = SettingService.Get(s => s.AchievementsAlert);
        }

        private StringNumber Timeout
        {
            get => (_timeout / 1000).ToString();
            set => SetTimeout(value);
        }

        private string TimeoutText => timeoutItems.FirstOrDefault(it => it.Value == Timeout).Key;

        private async void SetTimeout(StringNumber value)
        {
            if (_timeout == value)
            {
                return;
            }

            _timeout = value.ToInt32() * 1000;
            await SettingService.SetAsync(s => s.AlertTimeout, _timeout);
        }

        private void InitTimeoutItems()
        {
            for (int i = 0; i < 5; i++)
            {
                timeoutItems.Add($"{i + 1}s", (i + 1).ToString());
            }
        }
    }
}
