using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class DiarySetting : ImportantComponentBase
    {
        private bool title;

        private bool markdown;

        private bool imageLazy;

        private int editAutoSave;

        private bool showEditAutoSave;

        private bool showDiaryTimeFormat;

        private bool firstLineIndent;

        private bool codeLineNumber;

        private bool taskListLineThrough;

        private bool diaryIconText;

        private bool otherInfo;

        private bool autoPlay;

        private string? diaryTimeFormat;

        private readonly Dictionary<string, int> editAutoSaveItems = new()
        {
            {"Close" ,0},
            {"5s" ,5},
            {"15s" ,15},
            {"20s" ,20},
            {"30s" ,30},
            {"45s" ,45},
            {"60s" ,60},
        };

        private readonly static Dictionary<string, string> diaryTimeFormats = new()
        {
            { "Year/Month/Day Week","yyyy/MM/dd dddd" },
            { "Year/Month/Day Hour:Minute","yyyy/MM/dd HH:mm" },
            { "Year/Month/Day Hour:Minute Week","yyyy/MM/dd HH:mm dddd" },
        };

        protected override void ReadSettings()
        {
            base.ReadSettings();

            title = SettingService.Get(s => s.Title);
            markdown = SettingService.Get(s => s.Markdown);
            otherInfo = SettingService.Get(s => s.OtherInfo);
            diaryIconText = SettingService.Get(s => s.DiaryIconText);
            editAutoSave = SettingService.Get(s => s.EditAutoSave);
            imageLazy = SettingService.Get(s => s.ImageLazy);
            firstLineIndent = SettingService.Get(s => s.FirstLineIndent);
            codeLineNumber = SettingService.Get(s => s.CodeLineNumber);
            taskListLineThrough = SettingService.Get(s => s.TaskListLineThrough);
            autoPlay = SettingService.Get(s => s.AutoPlay);
            diaryTimeFormat = SettingService.Get(s => s.DiaryTimeFormat);
        }

        private string? EditAutoSaveText => I18n.T(editAutoSaveItems.FirstOrDefault(it => it.Value == editAutoSave).Key);

        private string DiaryTimeFormatKey => diaryTimeFormats.FirstOrDefault(x => x.Value == diaryTimeFormat).Key;

        private async Task UpdateEditAutoSaveSetting()
        {
            await SettingService.SetAsync(s => s.EditAutoSave, editAutoSave);
        }

        private async Task UpdateDiaryTimeFormatSetting(string value)
        {
            await SettingService.SetAsync(s => s.DiaryTimeFormat, value);
        }
    }
}
