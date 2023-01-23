using BlazorComponent;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;

namespace NoDecentDiary.Shared
{
    public partial class DiaryCardList : IDisposable
    {
        [Inject]
        public IPopupService? PopupService { get; set; }
        [Inject]
        public IDiaryService? DiaryService { get; set; }
        [Inject]
        public ITagService? TagService { get; set; }
        [Inject]
        public IDiaryTagService? DiaryTagService { get; set; }
        [Inject]
        public INavigateService? NavigateService { get; set; }

        [Parameter]
        public List<DiaryModel>? Value { get; set; }

        private bool _showDeleteDiary;
        private bool ShowDeleteDiary
        {
            get => _showDeleteDiary;
            set
            {
                _showDeleteDiary = value;
                if (!value)
                {
                    HandOnOKDelete = null;
                }
            }
        }
        private bool _showSelectTag;
        private bool ShowSelectTag
        {
            get => _showSelectTag;
            set
            {
                SetShowSelectTag(value);
            }
        }
        private int SelectedDiaryId;
        private List<TagModel> SelectedTags = new List<TagModel>();
        private Action? HandOnOKDelete;

        public DiaryCardList()
        {
            Value ??= new List<DiaryModel>();
        }

        private async Task Topping(DiaryModel diaryModel)
        {
            diaryModel.Top = !diaryModel.Top;
            await DiaryService!.UpdateAsync(diaryModel);

        }
        private void Delete(DiaryModel diaryModel)
        {
            HandOnOKDelete += async () =>
            {
                ShowDeleteDiary = false;
                bool flag = await DiaryService!.DeleteAsync(diaryModel);
                if (flag)
                {
                    Value!.Remove(diaryModel);
                    await PopupService!.AlertAsync("删除成功", AlertTypes.Success);
                    this.StateHasChanged();
                }
                else
                {
                    await PopupService!.AlertAsync("删除失败", AlertTypes.Error);
                }
            };
            ShowDeleteDiary= true;
            StateHasChanged();
        }
        private async void Copy(DiaryModel diaryModel)
        {
            var text = DiaryCopyContent(diaryModel);
            await Clipboard.Default.SetTextAsync(text);

            await PopupService!.AlertAsync(param =>
            {
                param.Content = "复制成功";
                param.Rounded = true;
                param.Type = AlertTypes.Success;
            });
        }
        private async Task Tag(int id)
        {
            SelectedDiaryId = id;
            SelectedTags = await TagService!.GetDiaryTagsAsync(SelectedDiaryId);
            StateHasChanged();
            ShowSelectTag = true;
        }
        private async Task HandOnSaveSelectTags()
        {
            await DiaryTagService!.DeleteAsync(it => it.DiaryId == SelectedDiaryId);
            await DiaryTagService.AddTagsAsync(SelectedDiaryId, SelectedTags);
            ShowSelectTag = false;
        }
        private void HandOnClick(int id)
        {
            NavigateService!.NavigateTo($"/Read/{id}");
        }
        private static string DiaryCopyContent(DiaryModel diary)
        {
            if (string.IsNullOrEmpty(diary.Title))
            {
                return diary.Content!;
            }
            return diary.Title + "\n" + diary.Content;
        }
        private void SetShowSelectTag(bool value)
        {
            if (_showSelectTag != value)
            {
                _showSelectTag = value;
                if (value)
                {
                    NavigateService!.Action += CloseSelectTag;
                }
                else
                {
                    NavigateService!.Action -= CloseSelectTag;
                }
            }
        }
        private void CloseSelectTag()
        {
            ShowSelectTag = false;
            StateHasChanged();
        }

        public void Dispose()
        {
            if (ShowSelectTag)
            {
                NavigateService!.Action -= CloseSelectTag;
            }
            GC.SuppressFinalize(this);
        }
    }
}
