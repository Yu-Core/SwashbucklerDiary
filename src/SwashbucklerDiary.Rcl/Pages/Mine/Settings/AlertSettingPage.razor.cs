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

        protected override async Task OnInitializedAsync()
        {
            SetTimeouts();
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await UpdateSettings();
                StateHasChanged();
            }
        }

        private StringNumber Timeout
        {
            get => (_timeout / 1000).ToString();
            set => SetTimeout(value);
        }

        private async Task UpdateSettings()
        {
            _timeout = await SettingService.Get<int>(Setting.AlertTimeout);
            achievementsAlert = await SettingService.Get<bool>(Setting.AchievementsAlert);
        }

        private async void SetTimeout(StringNumber value)
        {
            if(_timeout == value)
            {
                return;
            }

            _timeout = value.ToInt32() * 1000;
            AlertService.SetTimeout(_timeout);
            await SettingService.Set(Setting.AlertTimeout, _timeout);
        }

        private void SetTimeouts()
        {
            for (int i = 0; i < 5; i++)
            {
                timeoutItems.Add((i + 1).ToString(), string.Empty);
            }
        }
    }
}
