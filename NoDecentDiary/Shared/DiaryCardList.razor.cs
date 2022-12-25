using BlazorComponent;
using Masa.Blazor;
using Masa.Blazor.Popup.Components;
using Microsoft.AspNetCore.Components;
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

        [Parameter]
        [EditorRequired]
        public List<DiaryModel>? Value { get; set; }

        public DiaryCardList()
        {
            Value ??= new List<DiaryModel>();
        }

        private void Topping(DiaryModel diaryModel)
        {
            diaryModel.Top = !diaryModel.Top;
            this.StateHasChanged();
        }
        private async Task Delete(DiaryModel diaryModel)
        {
            var confirmed = await PopupService!.ConfirmAsync(param =>
            {
                param.Title = "删除日记";
                param.TitleStyle = "font-weight:700;";
                param.Content = "无法删除，因为每一篇日记都是珍贵的回忆。";
                param.IconColor = "red";
            });
            if (confirmed)
            {
                await PopupService!.AlertAsync(param =>
                {
                    param.Content = "昨日之日不可追，今日之日须臾期。";
                    param.Rounded = true;
                });
            }
            //Value!.Remove(diaryModel);
            //this.StateHasChanged();
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
    }
}
