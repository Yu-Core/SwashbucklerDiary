using BlazorComponent;
using Masa.Blazor;
using Masa.Blazor.Popup.Components;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.Extend;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;
using NoDecentDiary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Shared
{
    public partial class DiaryCardList
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
        [EditorRequired]
        public List<DiaryModel>? Value { get; set; }

        private bool showSelectTag;
        private int SelectedDiaryId;
        private List<TagModel> SelectedTags = new List<TagModel>();
        
        public DiaryCardList()
        {
            Value ??= new List<DiaryModel>();
        }

        private async Task Topping(DiaryModel diaryModel)
        {
            diaryModel.Top = !diaryModel.Top;
            await DiaryService!.UpdateAsync(diaryModel);

        }
        private async Task Delete(DiaryModel diaryModel)
        {
            var confirmed = await PopupService!.ConfirmAsync(param =>
            {
                param.Title = "删除日记";
                param.TitleStyle = "font-weight:700;";
                param.Content = "请慎重删除，每一篇日记都是珍贵的回忆。";
                param.IconColor = "red";
                param.ActionsStyle = "justify-content: flex-end;";
            });
            if (!confirmed)
            {
                return;
            }
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
            showSelectTag = true;
        }
        private async Task HandOnSaveSelectTags()
        {
            await DiaryTagService!.DeleteAsync(it => it.DiaryId == SelectedDiaryId);
            await DiaryTagService.AddTagsAsync(SelectedDiaryId, SelectedTags);
            showSelectTag = false;
        }
        private void HandOnClick(int id)
        {
            NavigateService!.NavigateTo($"/Read/{id}");
        }
        private string DiaryCopyContent(DiaryModel diary)
        {
            if (string.IsNullOrEmpty(diary.Title))
            {
                return diary.Content!;
            }
            return diary.Title + "\n" + diary.Content;
        }
    }
}
