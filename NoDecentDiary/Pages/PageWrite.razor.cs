using BlazorComponent;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;
using NoDecentDiary.Services;
using System.Diagnostics;

namespace NoDecentDiary.Pages
{
    public partial class PageWrite
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
        public NavigationManager? NavigationManager { get; set; }
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

        protected override Task OnInitializedAsync()
        {
            MasaBlazor!.Breakpoint.OnUpdate += () => { return InvokeAsync(this.StateHasChanged); };
            return base.OnInitializedAsync();
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
                NavigationManager!.NavigateTo("/");
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
                foreach (var item in SelectedTags)
                {
                    var record = new DiaryTagModel()
                    {
                        DiaryId = id,
                        TagId = item.Id
                    };
                    await DiaryTagService!.AddAsync(record);
                }
            }
            else
            {
                await PopupService!.AlertAsync("添加失败", AlertTypes.Error);
            }
            NavigationManager!.NavigateTo("/");
        }
    }
}
