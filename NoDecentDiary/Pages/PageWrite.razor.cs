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
        public INavigateService? NavigateService { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public int? TagId { get; set; }
        [Parameter]
        [SupplyParameterFromQuery]
        public int? DiaryId { get; set; }
        private readonly List<string> _weathers = new List<string>()
        {
            "晴","阴","小雨","中雨","大雨","小雪","中雪","大雪","雾",
        };
        private bool showTitle;
        private bool _showMenu;
        private bool ShowMenu
        {
            get => _showMenu;
            set
            {
                SetShowMenu(value);
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
        private DiaryModel Diary = new DiaryModel()
        {
            CreateTime = DateTime.Now,
            UpdateTime = DateTime.Now
        };
        private bool IsDesktop => MasaBlazor!.Breakpoint.SmAndUp;
        private List<TagModel> SelectedTags = new List<TagModel>();

        protected override async Task OnInitializedAsync()
        {
            MasaBlazor!.Breakpoint.OnUpdate += InvokeStateHasChangedAsync;
            NavigateService!.Action += HandOnBack;
            await SetTag();
            await SetDiary();
        }

        private async Task SetTag()
        {
            if (TagId != null)
            {
                var tag = await TagService!.FindAsync((int)TagId);
                if (tag != null)
                {
                    SelectedTags.Add(tag);
                }
            }
        }

        private async Task SetDiary()
        {
            if (DiaryId != null)
            {
                var diary = await DiaryService!.FindAsync((int)DiaryId);
                if (diary != null)
                {
                    Diary = diary;
                    showTitle = !string.IsNullOrEmpty(diary.Title);
                    SelectedTags = await TagService!.GetDiaryTagsAsync((int)DiaryId);
                }
            }
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
            ShowSelectTag = false;
        }

        private async Task HandOnSave()
        {
            if (string.IsNullOrWhiteSpace(Diary.Content))
            {
                return;
            }
            await SaveDiary();
        }

        private void HandOnBack()
        {
            if (string.IsNullOrWhiteSpace(Diary.Content))
            {
                NavigateToBack();
                return;
            }

            Task.Run(()=> SaveDiary());
        }

        private void HandOnClear()
        {
            Diary.Content = string.Empty;
            this.StateHasChanged();
        }

        private async Task SaveDiary()
        {
            if (DiaryId == null)
            {
                bool flag = await DiaryService!.AddAsync(Diary);
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
            }
            else
            {
                bool flag = await DiaryService!.UpdateAsync(Diary);
                if (flag)
                {
                    await PopupService!.AlertAsync("修改成功", AlertTypes.Success);
                    await DiaryTagService!.DeleteAsync(it => it.DiaryId == DiaryId);
                    await DiaryTagService!.AddTagsAsync((int)DiaryId, SelectedTags);
                }
                else
                {
                    await PopupService!.AlertAsync("修改失败", AlertTypes.Error);
                }
            }
            
            NavigateToBack();
        }

        public void NavigateToBack()
        {
            NavigateService!.NavigateToBack();
        }
        private async Task InvokeStateHasChangedAsync()
        {
            await InvokeAsync(StateHasChanged);
        }
        private void SetShowMenu(bool value)
        {
            if (_showMenu != value)
            {
                _showMenu = value;
                if (value)
                {
                    NavigateService!.Action += CloseMenu;
                }
                else
                {
                    NavigateService!.Action -= CloseMenu;
                }
            }
        }
        private void CloseMenu()
        {
            ShowMenu = false;
            StateHasChanged();
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
            MasaBlazor!.Breakpoint.OnUpdate -= InvokeStateHasChangedAsync;
            if (ShowMenu)
            {
                NavigateService!.Action -= CloseMenu;
            }
            if (ShowSelectTag)
            {
                NavigateService!.Action -= CloseSelectTag;
            }
            NavigateService!.Action -= HandOnBack;
            GC.SuppressFinalize(this);
        }
    }
}
