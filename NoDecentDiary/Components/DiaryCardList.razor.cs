using BlazorComponent;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;

namespace NoDecentDiary.Components
{
    public partial class DiaryCardList : MyComponentBase
    {
        private bool ShowDeleteDiary;
        private bool ShowSelectTag;
        private int SelectedDiaryId;
        private List<TagModel> SelectedTags = new();
        private Action? OnDelete;

        [Inject]
        public IDiaryService DiaryService { get; set; } = default!;
        [Inject]
        public ITagService TagService { get; set; } = default!;
        [Inject]
        public IDiaryTagService DiaryTagService { get; set; } = default!;
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

        private async Task OnTopping(DiaryModel diaryModel)
        {
            diaryModel.Top = !diaryModel.Top;
            await DiaryService!.UpdateAsync(diaryModel);
        }

        private void OpenDeleteDialog(DiaryModel diaryModel)
        {
            OnDelete = null;
            OnDelete += async () =>
            {
                ShowDeleteDiary = false;
                bool flag = await DiaryService!.DeleteAsync(diaryModel);
                if (flag)
                {
                    Value!.Remove(diaryModel);
                    await PopupService.ToastAsync(it =>
                    {
                        it.Type = AlertTypes.Success;
                        it.Title = I18n!.T("Share.DeleteSuccess");
                    });
                    StateHasChanged();
                }
                else
                {
                    await PopupService.ToastAsync(it =>
                    {
                        it.Type = AlertTypes.Error;
                        it.Title = I18n!.T("Share.DeleteFail");
                    });
                }
                await OnDeleted.InvokeAsync();
            };
            ShowDeleteDiary = true;
            StateHasChanged();
        }

        private async Task OnCopy(DiaryModel diaryModel)
        {
            var text = DiaryCopyContent(diaryModel);
            await SystemService.SetClipboard(text);

            await PopupService.ToastAsync(it =>
            {
                it.Type = AlertTypes.Success;
                it.Title = I18n!.T("Share.CopySuccess");
            });
        }

        private async Task OnTag(int id)
        {
            SelectedDiaryId = id;
            SelectedTags = await TagService!.GetDiaryTagsAsync(SelectedDiaryId);
            StateHasChanged();
            ShowSelectTag = true;
        }

        private async Task SaveSelectTags()
        {
            await DiaryTagService!.DeleteAsync(it => it.DiaryId == SelectedDiaryId);
            await DiaryTagService.AddTagsAsync(SelectedDiaryId, SelectedTags);
            ShowSelectTag = false;
        }

        private void OnClick(int id)
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
    }
}
