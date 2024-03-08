using BlazorComponent;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Shared;

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

        protected override void UpdateSettings()
        {
            base.UpdateSettings();

            _timeout = SettingService.Get<int>(Setting.AlertTimeout);
            achievementsAlert = SettingService.Get<bool>(Setting.AchievementsAlert);
        }

        private StringNumber Timeout
        {
            get => (_timeout / 1000).ToString();
            set => SetTimeout(value);
        }

        private async void SetTimeout(StringNumber value)
        {
            if (_timeout == value)
            {
                return;
            }

            _timeout = value.ToInt32() * 1000;
            AlertService.SetTimeout(_timeout);
            await SettingService.Set(Setting.AlertTimeout, _timeout);
        }

        private void InitTimeoutItems()
        {
            for (int i = 0; i < 5; i++)
            {
                timeoutItems.Add((i + 1).ToString(), string.Empty);
            }
        }
    }
}
