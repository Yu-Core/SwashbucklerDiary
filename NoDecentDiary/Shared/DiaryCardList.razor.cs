using BlazorComponent;
using BlazorComponent.I18n;
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
        [Inject]
        private I18n? I18n { get; set; }
        [Inject]
        private ISystemService? SystemService { get; set; }

        [Parameter]
        public List<DiaryModel>? Value { get; set; }
        [Parameter]
        public string? Class { get; set; }

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
                    await PopupService!.ToastAsync(it => 
                    { 
                        it.Type = AlertTypes.Success; 
                        it.Title = I18n!.T("Share.DeleteSuccess"); 
                    });
                    this.StateHasChanged();
                }
                else
                {
                    await PopupService!.ToastAsync(it => 
                    { 
                        it.Type = AlertTypes.Error;
                        it.Title = I18n!.T("Share.DeleteFail");
                    });
                }
            };
            ShowDeleteDiary= true;
            StateHasChanged();
        }
        private async void Copy(DiaryModel diaryModel)
        {
            var text = DiaryCopyContent(diaryModel);
            await SystemService!.SetClipboard(text);

            await PopupService!.ToastAsync(it =>
            {
                it.Type = AlertTypes.Success;
                it.Title = I18n!.T("Share.CopySuccess");
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
