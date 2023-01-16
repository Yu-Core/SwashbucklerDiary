using BlazorComponent;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.Interface;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Pages
{
    public partial class PageRead : INavigateToBack, IDisposable
    {
        [Inject]
        public MasaBlazor? MasaBlazor { get; set; }
        [Inject]
        public IDiaryService? DiaryService { get; set; }
        [Inject]
        public IPopupService? PopupService { get; set; }
        [Inject]
        public IDiaryTagService? DiaryTagService { get; set; }
        [Inject]
        public ITagService? TagService { get; set; }
        [Inject]
        public NavigationManager? Navigation { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public string? Href { get; set; }
        [Parameter]
        public int Id { get; set; }
        private bool showMenu;
        private bool ShowTitle => !string.IsNullOrEmpty(_diary.Title);
        private bool ShowWeather => !string.IsNullOrEmpty(_diary.Weather);
        private DiaryModel _diary = new DiaryModel();
        private bool IsDesktop => MasaBlazor!.Breakpoint.SmAndUp;
        private List<TagModel> SelectedTags = new List<TagModel>();

        protected override async Task OnInitializedAsync()
        {
            await UpdateDiary();
            await UpdateTag();
            MasaBlazor!.Breakpoint.OnUpdate += InvokeStateHasChangedAsync;
        }

        private async Task UpdateDiary()
        {
            var diaryModel = await DiaryService!.FindAsync(Id);
            if (diaryModel == null)
            {
                NavigateToBack();
                return;
            }
            _diary = diaryModel;
        }
        private async Task UpdateTag()
        {
            SelectedTags = await TagService!.GetDiaryTagsAsync(Id);
        }
        private void HandOnBack()
        {
            NavigateToBack();
        }
        public void NavigateToBack()
        {
            this.DefaultNavigateToBack();
        }
        private async Task InvokeStateHasChangedAsync()
        {
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            MasaBlazor!.Breakpoint.OnUpdate -= InvokeStateHasChangedAsync;
            GC.SuppressFinalize(this);
        }
        public async Task HandOnDelete()
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
            bool flag = await DiaryService!.DeleteAsync(Id);
            if (flag)
            {
                await PopupService!.AlertAsync("删除成功", AlertTypes.Success);
            }
            else
            {
                await PopupService!.AlertAsync("删除失败", AlertTypes.Error);
            }
            NavigateToBack();
        }
    }
}
