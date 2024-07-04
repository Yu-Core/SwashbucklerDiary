using Masa.Blazor;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Shared;

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

            title = SettingService.Get<bool>(Setting.Title);
            markdown = SettingService.Get<bool>(Setting.Markdown);
            editAutoSave = SettingService.Get<int>(Setting.EditAutoSave);
            imageLazy = SettingService.Get<bool>(Setting.ImageLazy);
            firstLineIndent = SettingService.Get<bool>(Setting.FirstLineIndent);
            codeLineNumber = SettingService.Get<bool>(Setting.CodeLineNumber);
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
            await SettingService.Set(Setting.EditAutoSave, editAutoSave);
        }
    }
}
