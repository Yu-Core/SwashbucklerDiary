using Masa.Blazor;
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

        private bool firstLineIndent;

        private bool codeLineNumber;

        private bool taskListLineThrough;

        private bool diaryIconText;

        private readonly Dictionary<string, string> editAutoSaveItems = new()
        {
            {"Setting.Display.Diary.EditAutoSave.Close" ,"0"},
            {"5s" ,"5"},
            {"15s" ,"15"},
            {"20s" ,"20"},
            {"30s" ,"30"},
            {"45s" ,"45"},
            {"60s" ,"60"},

        };

        protected override void ReadSettings()
        {
            base.ReadSettings();

            title = SettingService.Get(s => s.Title);
            markdown = SettingService.Get(s => s.Markdown);
            diaryIconText = SettingService.Get(s => s.DiaryIconText);
            editAutoSave = SettingService.Get(s => s.EditAutoSave);
            imageLazy = SettingService.Get(s => s.ImageLazy);
            firstLineIndent = SettingService.Get(s => s.FirstLineIndent);
            codeLineNumber = SettingService.Get(s => s.CodeLineNumber);
            taskListLineThrough = SettingService.Get(s => s.TaskListLineThrough);
        }

        private StringNumber EditAutoSave
        {
            get => editAutoSave.ToString();
            set => SetEditAutoSave(value);
        }

        private string? EditAutoSaveText => I18n.T(editAutoSaveItems.FirstOrDefault(it => it.Value == EditAutoSave).Key);

        private async void SetEditAutoSave(StringNumber value)
        {
            if (editAutoSave == value)
            {
                return;
            }

            editAutoSave = value.ToInt32();
            await SettingService.SetAsync(s => s.EditAutoSave, editAutoSave);
        }
    }
}
