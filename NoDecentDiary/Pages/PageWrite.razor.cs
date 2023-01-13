using BlazorComponent;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.Maui.Controls;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;
using NoDecentDiary.Services;
using System.Diagnostics;

namespace NoDecentDiary.Pages
{
    public partial class PageWrite : IDisposable
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
        public int? TagId { get; set; }
        private readonly List<string> _weathers = new List<string>()
        {
            "晴","阴","小雨","中雨","大雨","小雪","中雪","大雪","雾",
        };
        private bool showMenu;
        private bool showTitle;
        private bool showSelectTag;
        private DiaryModel _diary = new DiaryModel();
        private bool IsDesktop => MasaBlazor!.Breakpoint.SmAndUp;
        private List<TagModel> SelectedTags = new List<TagModel>();

        protected override async Task OnInitializedAsync()
        {
            MasaBlazor!.Breakpoint.OnUpdate += InvokeStateHasChangedAsync;
            await SetTag();
        }

        private void RemoveSelectedTag(TagModel tag)
        {
            int index = SelectedTags.IndexOf(tag);
            if (index > -1)
            {
                SelectedTags.RemoveAt(index);
            }
        }

        private void HandOnSaveSelectTags()
        {
            showSelectTag = false;
        }

        private async Task HandOnSave()
        {
            if (string.IsNullOrWhiteSpace(_diary.Content))
            {
                return;
            }
            await AddDiary();
        }

        private async Task HandOnBack()
        {
            if (string.IsNullOrWhiteSpace(_diary.Content))
            {
                NavigationBack();
                return;
            }
            await AddDiary();
        }

        private void HandOnClear()
        {
            _diary.Content = string.Empty;
            this.StateHasChanged();
        }

        private async Task AddDiary()
        {
            _diary.CreateTime = DateTime.Now;
            _diary.UpdateTime = DateTime.Now;
            bool flag = await DiaryService!.AddAsync(_diary);
            if (flag)
            {
                await PopupService!.AlertAsync("添加成功", AlertTypes.Success);
                int id = await DiaryService.GetLastInsertRowId();
                await DiaryTagService!.AddTagsAsync(id, SelectedTags);
            }
            else
            {
                await PopupService!.AlertAsync("添加失败", AlertTypes.Error);
            }
            NavigationBack();
        }
        private async Task SetTag()
        {
            if(TagId != null)
            {
                var tag = await TagService!.FindAsync((int)TagId);
                if (tag != null)
                {
                    SelectedTags.Add(tag);
                }
            }
        }
        private void NavigationBack()
        {
            if (TagId != null)
            {
                Navigation!.NavigateTo($"/Tag/{TagId}");
            }
            else
            {
                Navigation!.NavigateTo("/");
            }
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
    }
}
