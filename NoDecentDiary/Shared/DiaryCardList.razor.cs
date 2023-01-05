using BlazorComponent;
using Masa.Blazor;
using Masa.Blazor.Popup.Components;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;
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
            //var confirmed = await PopupService!.ConfirmAsync(param =>
            //{
            //    param.Title = "删除日记";
            //    param.TitleStyle = "font-weight:700;";
            //    param.Content = "无法删除，因为每一篇日记都是珍贵的回忆。";
            //    param.IconColor = "red";
            //});
            //if (confirmed)
            //{
            //    await PopupService!.AlertAsync(param =>
            //    {
            //        param.Content = "昨日之日不可追，今日之日须臾期。";
            //        param.Rounded = true;
            //    });
            //}

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
            await Clipboard.Default.SetTextAsync(diaryModel.Content);

            await PopupService!.AlertAsync(param =>
            {
                param.Content = "复制成功";
                param.Rounded = true;
                param.Type = AlertTypes.Success;
            });
        }
        private async Task Tag(DiaryModel diaryModel)
        {
            SelectedDiaryId = diaryModel.Id;
            SelectedTags = await DiaryService!.GetTagsAsync(SelectedDiaryId);
            showSelectTag = true;
        }
        private async Task HandOnSaveSelectTags()
        {

            foreach (var item in SelectedTags)
            {
                var record = new DiaryTagModel()
                {
                    DiaryId = SelectedDiaryId,
                    TagId = item.Id
                };
                await DiaryTagService!.AddAsync(record);
            }
        }
    }
}
