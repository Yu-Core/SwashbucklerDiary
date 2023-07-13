using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class DiaryCardList : MyComponentBase
    {
        private List<DiaryModel> _value = new();
        private bool ShowDeleteDiary;
        private bool ShowSelectTag;
        private bool ShowExport;
        private bool ShowPrivacy;
        private bool ShowIcon;
        private DiaryModel SelectedDiary = new();
        private List<DiaryModel> ExportDiaries = new();

        [Inject]
        public IDiaryService DiaryService { get; set; } = default!;
        [Inject]
        private IPlatformService PlatformService { get; set; } = default!;

        [Inject]
        protected ISettingsService SettingsService { get; set; } = default!;

        [Parameter]
        public List<DiaryModel> Value
        {
            get => _value.OrderByDescending(it => it.Top).ToList();
            set => _value = value;
        }
        [Parameter]
        public string? Class { get; set; }
        [Parameter]
        public string? CardClass { get; set; }
        [Parameter]
        public EventCallback OnUpdate { get; set; }
        [Parameter]
        public List<TagModel> Tags { get; set; } = new();
        [Parameter]
        public EventCallback<List<TagModel>> TagsChanged { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await base.OnInitializedAsync();
        }

        private List<TagModel> SelectedTags
        {
            get => SelectedDiary.Tags ?? new();
            set => SelectedDiary.Tags = value;
        }

        private async Task LoadSettings()
        {
            ShowPrivacy = await SettingsService.Get(SettingType.PrivacyMode);
            ShowIcon = await SettingsService.Get(SettingType.DiaryCardIcon);
        }

        private async Task HandleTopping(DiaryModel diaryModel)
        {
            diaryModel.Top = !diaryModel.Top;
            await DiaryService.UpdateAsync(diaryModel);
        }

        private void OpenDeleteDialog(DiaryModel diaryModel)
        {
            SelectedDiary = diaryModel;
            ShowDeleteDiary = true;
        }

        private async Task HandleDelete(DiaryModel diaryModel)
        {
            ShowDeleteDiary = false;
            bool flag = await DiaryService.DeleteAsync(diaryModel);
            if (flag)
            {
                var index = _value.FindIndex(it=>it.Id == diaryModel.Id);
                if (index < 0)
                {
                    return;
                }
                _value.RemoveAt(index);
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
                StateHasChanged();
            }
            else
            {
                await AlertService.Error(I18n.T("Share.DeleteFail"));
            }
            await OnUpdate.InvokeAsync();
        }

        private async Task HandleCopy(DiaryModel diaryModel)
        {
            var text = DiaryCopyContent(diaryModel);
            await PlatformService.SetClipboard(text);

            await AlertService.Success(I18n.T("Share.CopySuccess"));
        }

        private async Task HandleTag(DiaryModel diary)
        {
            SelectedDiary = diary;
            SelectedTags = await DiaryService.GetTagsAsync(SelectedDiary.Id);
            StateHasChanged();
            ShowSelectTag = true;
        }

        private async Task SaveSelectTags()
        {
            await DiaryService.UpdateTagsAsync(SelectedDiary);
            ShowSelectTag = false;
        }

        private void HandleClick(DiaryModel diaryModel)
        {
            NavigateService.NavigateTo($"/read/{diaryModel.Id}");
        }

        private static string DiaryCopyContent(DiaryModel diary)
        {
            if (string.IsNullOrEmpty(diary.Title))
            {
                return diary.Content!;
            }
            return diary.Title + "\n" + diary.Content;
        }

        private void OpenExportDialog(DiaryModel diary)
        {
            ExportDiaries = new() { diary };
            ShowExport = true;
        }

        private async Task HandlePrivacy(DiaryModel diaryModel)
        {
            diaryModel.Private = !diaryModel.Private;
            await DiaryService.UpdateAsync(diaryModel);
            var diary = _value.FirstOrDefault(it => it.Id == diaryModel.Id);
            if (diary == null)
            {
                return;
            }
            _value.Remove(diary);
            await OnUpdate.InvokeAsync();
        }
    }
}
