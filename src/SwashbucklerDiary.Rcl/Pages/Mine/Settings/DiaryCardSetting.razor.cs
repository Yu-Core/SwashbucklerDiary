using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class DiaryCardSetting : ImportantComponentBase
    {
        private bool diaryCardIcon;

        private bool diaryCardTags;

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

            diaryCardIcon = SettingService.Get(s => s.DiaryCardIcon);
            diaryCardTags = SettingService.Get(s => s.DiaryCardTags);
            diaryCardDateFormat = SettingService.Get(s => s.DiaryCardDateFormat);
        }

        private string DiaryCardDateFormatKey => diaryCardDateFormats.FirstOrDefault(x => x.Value == diaryCardDateFormat).Key;

        private async Task DiaryCardDateFormatChanged(string value)
        {
            await SettingService.SetAsync(s => s.DiaryCardDateFormat, value);
        }
    }
}
