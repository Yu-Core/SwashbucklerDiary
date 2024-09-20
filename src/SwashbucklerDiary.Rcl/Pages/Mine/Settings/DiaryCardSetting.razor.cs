using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class DiaryCardSetting : ImportantComponentBase
    {
        private bool diaryCardIcon;

        private bool diaryCardTags;

        private string? DiaryCardTimeFormat;

        private bool showDiaryCardTimeFormat;

        private readonly static Dictionary<string, string> DiaryCardTimeFormats = new()
        {
            {"DateTimeFormat.MM/dd","MM/dd" },
            {"DateTimeFormat.yyyy/MM/dd","yyyy/MM/dd" },
            {"DateTimeFormat.MM/dd HH:mm","MM/dd HH:mm" },
            {"DateTimeFormat.yyyy/MM/dd HH:mm","yyyy/MM/dd HH:mm" },
            {"DateTimeFormat.MM/dd HH:mm dddd","MM/dd HH:mm dddd" },
            {"DateTimeFormat.yyyy/MM/dd HH:mm dddd","yyyy/MM/dd HH:mm dddd" },
        };

        protected override void ReadSettings()
        {
            base.ReadSettings();

            diaryCardIcon = SettingService.Get(s => s.DiaryCardIcon);
            diaryCardTags = SettingService.Get(s => s.DiaryCardTags);
            DiaryCardTimeFormat = SettingService.Get(s => s.DiaryCardTimeFormat);
        }

        private string DiaryCardTimeFormatKey => DiaryCardTimeFormats.FirstOrDefault(x => x.Value == DiaryCardTimeFormat).Key;

        private async Task DiaryCardTimeFormatChanged(string value)
        {
            await SettingService.SetAsync(s => s.DiaryCardTimeFormat, value);
        }
    }
}
