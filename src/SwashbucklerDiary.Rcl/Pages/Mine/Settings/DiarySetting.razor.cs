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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await UpdateSettings();
                StateHasChanged();
            }
        }

        private StringNumber EditAutoSave
        {
            get => editAutoSave.ToString();
            set => SetEditAutoSave(value);
        }

        private async Task UpdateSettings()
        {
            var titleTask = SettingService.Get<bool>(Setting.Title);
            var markdownTask = SettingService.Get<bool>(Setting.Markdown);
            var editAutoSaveTask = SettingService.Get<int>(Setting.EditAutoSave);
            await Task.WhenAll(titleTask, markdownTask, editAutoSaveTask);
            title = titleTask.Result;
            markdown = markdownTask.Result;
            editAutoSave = editAutoSaveTask.Result;
        }

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
            for (int i = 0; i < 6; i++)
            {
                editAutoSaveItems.Add(((i + 1) * 10).ToString(), string.Empty);
            }
        }
    }
}
