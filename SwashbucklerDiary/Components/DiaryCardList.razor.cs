using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class DiaryCardList : MyComponentBase
    {
        private bool ShowDeleteDiary;
        private bool ShowSelectTag;
        private bool ShowExport;
        private DiaryModel SelectDiary = new();
        private List<DiaryModel> ExportDiaries = new();

        [Inject]
        public IDiaryService DiaryService { get; set; } = default!;
        [Inject]
        private ISystemService SystemService { get; set; } = default!;

        [Parameter]
        public List<DiaryModel> Value { get; set; } = new List<DiaryModel>();
        [Parameter]
        public string? Class { get; set; }
        [Parameter]
        public string? CardClass { get; set; }
        [Parameter]
        public EventCallback OnDeleted { get; set; }

        private List<TagModel> SelectedTags
        {
            get=> SelectDiary.Tags ?? new ();
            set => SelectDiary.Tags = value;
        }

        private async Task HandleTopping(DiaryModel diaryModel)
        {
            diaryModel.Top = !diaryModel.Top;
            await DiaryService.UpdateAsync(diaryModel);
        }

        private void OpenDeleteDialog(DiaryModel diaryModel)
        {
            SelectDiary = diaryModel;
            ShowDeleteDiary = true;
        }

        private async Task HandleDelete(DiaryModel diaryModel)
        {
            ShowDeleteDiary = false;
            bool flag = await DiaryService.DeleteAsync(diaryModel);
            if (flag)
            {
                Value!.Remove(diaryModel);
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
                StateHasChanged();
            }
            else
            {
                await AlertService.Error(I18n.T("Share.DeleteFail"));
            }
            await OnDeleted.InvokeAsync();
        }

        private async Task HandleCopy(DiaryModel diaryModel)
        {
            var text = DiaryCopyContent(diaryModel);
            await SystemService.SetClipboard(text);

            await AlertService.Success(I18n.T("Share.CopySuccess"));
        }

        private async Task HandleTag(DiaryModel diary)
        {
            SelectDiary = diary;
            SelectedTags = await DiaryService.GetTagsAsync(SelectDiary.Id);
            StateHasChanged();
            ShowSelectTag = true;
        }

        private async Task SaveSelectTags()
        {
            await DiaryService.UpdateTagsAsync(SelectDiary);
            ShowSelectTag = false;
        }

        private void HandleClick(Guid id)
        {
            NavigateService.NavigateTo($"/read/{id}");
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
        
    }
}
