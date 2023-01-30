using BlazorComponent;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using NoDecentDiary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Pages
{
    public partial class PageMine
    {
        [Inject]
        public INavigateService? NavigateService { get; set; }
        [Inject]
        private IDiaryService? DiaryService { get; set; }
        [Inject]
        private IPopupService? PopupService { get; set; }

        private int DiaryCount { get; set; }
        private long WordCount { get; set; }
        private int ActiveDayCount { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await SetCount();
        }

        private async Task SetCount()
        {
            DiaryCount = await DiaryService!.CountAsync();
            var diaries = await DiaryService!.QueryAsync();
            foreach (var item in diaries)
            {
                WordCount += item.Content?.Length ?? 0;
            }
            ActiveDayCount = diaries.Select(it => it.CreateTime).Distinct().Count();
        }
        private Task ToDo()
        {
            return PopupService!.ToastAsync(it =>
            {
                it.Type = AlertTypes.Info;
                it.Title = "该功能暂未开放";
                it.Content= "敬请期待";
            });
        }
        private void NavigateToSearch()
        {
            NavigateService!.NavigateTo("/Search");
        }
    }
}
