using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class DiaryCardSetting : ImportantComponentBase
    {
        private bool diaryCardIcon;

        private string? diaryCardDateFormat;

        private bool showDiaryCardDateFormat;

        private readonly static Dictionary<string, string> diaryCardDateFormats = new()
        {
            {"DateTimeFormat.MM/dd","MM/dd" },
            {"DateTimeFormat.yyyy/MM/dd","yyyy/MM/dd" },
        };

        protected override void ReadSettings()
        {
            base.ReadSettings();

            diaryCardIcon = SettingService.Get<bool>(Setting.DiaryCardIcon);
            diaryCardDateFormat = SettingService.Get<string>(Setting.DiaryCardDateFormat);
        }

        private string DiaryCardDateFormatKey => diaryCardDateFormats.FirstOrDefault(x => x.Value == diaryCardDateFormat).Key;

        private async Task DiaryCardDateFormatChanged(string value)
        {
            await SettingService.Set(Setting.DiaryCardDateFormat, value);
        }
    }
}
