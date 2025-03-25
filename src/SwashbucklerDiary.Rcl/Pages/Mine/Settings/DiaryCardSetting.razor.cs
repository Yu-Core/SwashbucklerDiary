using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class DiaryCardSetting : ImportantComponentBase
    {
        private bool diaryCardIcon;

        private bool diaryCardTags;

        private bool diaryCardLocation;

        private string? DiaryCardTimeFormat;

        private bool showDiaryCardTimeFormat;

        private readonly static Dictionary<string, string> DiaryCardTimeFormats = new()
        {
            {"Month/Day","MM/dd" },
            {"Year/Month/Day","yyyy/MM/dd" },
            {"Month/Day Hour:Minute","MM/dd HH:mm" },
            {"Year/Month/Day Hour:Minute","yyyy/MM/dd HH:mm" },
            {"Month/Day Hour:Minute Week","MM/dd HH:mm dddd" },
            {"Year/Month/Day Hour:Minute Week","yyyy/MM/dd HH:mm dddd" },
        };

        protected override void ReadSettings()
        {
            base.ReadSettings();

            diaryCardIcon = SettingService.Get(s => s.DiaryCardIcon);
            diaryCardTags = SettingService.Get(s => s.DiaryCardTags);
            diaryCardLocation = SettingService.Get(s => s.DiaryCardLocation);
            DiaryCardTimeFormat = SettingService.Get(s => s.DiaryCardTimeFormat);
        }

        private string DiaryCardTimeFormatKey => DiaryCardTimeFormats.FirstOrDefault(x => x.Value == DiaryCardTimeFormat).Key;

        private async Task DiaryCardTimeFormatChanged(string value)
        {
            await SettingService.SetAsync(s => s.DiaryCardTimeFormat, value);
        }
    }
}
