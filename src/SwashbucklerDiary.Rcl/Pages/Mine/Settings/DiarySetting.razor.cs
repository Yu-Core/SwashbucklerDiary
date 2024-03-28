using BlazorComponent;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class DiarySetting : ImportantComponentBase
    {
        private bool title;

        private bool markdown;

        private int editAutoSave;

        private bool showEditAutoSave;

        private readonly Dictionary<string, string> editAutoSaveItems = [];

        protected override void OnInitialized()
        {
            base.OnInitialized();

            InitEditAutoSaveItems();
        }

        protected override void UpdateSettings()
        {
            base.UpdateSettings();

            title = SettingService.Get<bool>(Setting.Title);
            markdown = SettingService.Get<bool>(Setting.Markdown);
            editAutoSave = SettingService.Get<int>(Setting.EditAutoSave);
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

        private void InitEditAutoSaveItems()
        {
            editAutoSaveItems.Add("Setting.Display.Diary.EditAutoSave.Close", "0");
            for (int i = 0; i < 6; i++)
            {
                editAutoSaveItems.Add($"{(i + 1) * 10}s", ((i + 1) * 10).ToString());
            }
        }
    }
}
