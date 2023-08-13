using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class DiaryCardSetting : PageComponentBase
    {
        private bool DiaryCardIcon;
        private string? DiaryCardDateFormat;
        private bool ShowDiaryCardDateFormat;
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

        private string MRadioColor => ThemeService.Dark ? "white" : "black";
        private string DiaryCardDateFormatKey => DiaryCardDateFormats.FirstOrDefault(x => x.Value == DiaryCardDateFormat).Key;

        private async Task LoadSettings()
        {
            DiaryCardIcon = await SettingsService.Get(SettingType.DiaryCardIcon);
            DiaryCardDateFormat = await SettingsService.Get(SettingType.DiaryCardDateFormat);
        }

        private async Task DiaryCardDateFormatChanged(string value)
        {
            DiaryCardDateFormat = value;
            await SettingsService.Save(SettingType.DiaryCardDateFormat, value);
        }
    }
}
