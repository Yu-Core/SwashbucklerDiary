using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class DiaryCardSetting : ImportantComponentBase
    {
        private bool diaryCardIcon;

        private string? diaryCardDateFormat;

        private bool showDiaryCardDateFormat;

        private readonly static Dictionary<string, string> DiaryCardDateFormats = new()
        {
            {"DateTimeFormat.MM/dd","MM/dd" },
            {"DateTimeFormat.yyyy/MM/dd","yyyy/MM/dd" },
        };

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await base.OnInitializedAsync();
        }

        private string DiaryCardDateFormatKey => DiaryCardDateFormats.FirstOrDefault(x => x.Value == diaryCardDateFormat).Key;

        private async Task LoadSettings()
        {
            diaryCardIcon = await Preferences.Get<bool>(Setting.DiaryCardIcon);
            diaryCardDateFormat = await Preferences.Get<string>(Setting.DiaryCardDateFormat);
        }

        private async Task DiaryCardDateFormatChanged(string value)
        {
            diaryCardDateFormat = value;
            await Preferences.Set(Setting.DiaryCardDateFormat, value);
        }
    }
}
