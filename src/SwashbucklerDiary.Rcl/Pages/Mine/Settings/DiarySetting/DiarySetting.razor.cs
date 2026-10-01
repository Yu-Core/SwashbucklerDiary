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

        private bool showDiaryInsertTimeFormat;

        private bool firstLineIndent;

        private bool codeLineNumber;

        private bool taskListLineThrough;

        private bool diaryIconText;

        private bool otherInfo;

        private bool autoPlay;

        private bool linkCard;

        private bool originalFileName;

        private string? letterPaperSealText;

        private bool showEditLetterPaperSealText;

        private string? diaryTimeFormat;

        private string? diaryInsertTimeFormat;

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

        private static readonly Dictionary<string, string> diaryTimeFormats = new()
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
            diaryInsertTimeFormat = SettingService.Get(s => s.DiaryInsertTimeFormat);
            linkCard = SettingService.Get(s => s.LinkCard);
            originalFileName = SettingService.Get(s => s.OriginalFileName);
            letterPaperSealText = SettingService.Get(s => s.LetterPaperSealText);
        }

        private string LetterPaperSealTextDisplay
            => string.IsNullOrWhiteSpace(letterPaperSealText) ? I18n.T("Please enter 1-2 Chinese characters") : letterPaperSealText!;

        private static bool IsCjkChar(char c)
            => c is >= (char)0x4E00 and <= (char)0x9FFF or >= (char)0x3400 and <= (char)0x4DBF;

        private async Task SaveLetterPaperSealText(string? value)
        {
            var text = (value ?? string.Empty).Trim();

            // 空串表示清空、恢复各模板默认章文；非空则须为 1–2 个汉字
            if (text.Length > 0 && (text.Any(c => !IsCjkChar(c)) || text.Length > 2))
            {
                await AlertService.ErrorAsync(I18n.T("Please enter 1-2 Chinese characters"));
                return;
            }

            showEditLetterPaperSealText = false;
            letterPaperSealText = text;
            await SettingService.SetAsync(s => s.LetterPaperSealText, text);
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

        private async Task UpdateDiaryInsertTimeFormatSetting()
        {
            await SettingService.SetAsync(s => s.DiaryInsertTimeFormat, diaryInsertTimeFormat);
        }

        private async Task ResetDiaryInsertTimeFormatSetting()
        {
            showDiaryInsertTimeFormat = false;
            await SettingService.RemoveAsync(it => it.DiaryInsertTimeFormat);
            diaryInsertTimeFormat = SettingService.Get(s => s.DiaryInsertTimeFormat);
        }
    }
}
