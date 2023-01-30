using BlazorComponent;
using BlazorComponent.I18n;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;
using NoDecentDiary.Shared;
using System;

namespace NoDecentDiary.Pages
{
    public partial class Index : IDisposable
    {
        [Inject]
        public I18n? I18n { get; set; }
        [Inject]
        public IDiaryService? DiaryService { get; set; }
        [Inject]
        public ITagService? TagService { get; set; }
        [Inject]
        public IDiaryTagService? DiaryTagService { get; set; }
        [Inject]
        public IPopupService? PopupService { get; set; }
        [Inject]
        public INavigateService? NavigateService { get; set; }
        [Inject]
        public NavigationManager? Navigation { get; set; }
        [CascadingParameter]
        public Error? Error { get; set; }
        [Parameter]
        [SupplyParameterFromQuery]
        public string? Type { get; set; }
        private StringNumber tabs = 0;
        private string? AddTagName;
        private bool _showAddTag;
        private bool ShowAddTag
        {
            get => _showAddTag;
            set
            {
                SetShowAddTag(value);
            }
        }
        private List<DiaryModel> Diaries { get; set; } = new List<DiaryModel>();
        private List<TagModel> Tags { get; set; } = new List<TagModel>();
        private readonly List<string> Types = new()
        {
            "All", "Tags"
        };

        protected override async Task OnInitializedAsync()
        {
            SetTab();
            await UpdateTags();
            await UpdateDiaries();
        }
        private async Task UpdateDiaries()
        {
            var diaryModels = await DiaryService!.QueryAsync();
            Diaries = diaryModels.Take(50).ToList();
        }
        private async Task UpdateTags()
        {
            Tags = await TagService!.QueryAsync();
        }
        private void SetTab()
        {
            if(string.IsNullOrEmpty(Type))
            {
                Type = Types[0];
            }
            tabs = Types.IndexOf(Type!);
        }
        private async Task HandOnRefreshData(StringNumber value)
        {
            if (value == 0)
            {
                await UpdateDiaries();
            }
            else if (value == 1)
            {
                await UpdateTags();
            }
            var url = Navigation!.GetUriWithQueryParameter("Type", Types[tabs.ToInt32()]);
            Navigation!.NavigateTo(url);
        }
        private async Task HandOnSaveAddTag()
        {
            ShowAddTag = false;
            if (string.IsNullOrWhiteSpace(AddTagName))
            {
                return;
            }

            if (Tags.Any(it => it.Name == AddTagName))
            {
                await PopupService!.ToastAsync(it =>
                {
                    it.Type = AlertTypes.Warning;
                    it.Title = "标签已存在";
                    it.Content = "请勿重复添加";
                });
                return;
            }

            TagModel tagModel = new TagModel()
            {
                Name = AddTagName
            };
            bool flag = await TagService!.AddAsync(tagModel);
            if (!flag)
            {
                await PopupService!.ToastAsync(it =>
                {
                    it.Type = AlertTypes.Error;
                    it.Title = "添加失败";
                });
                return;
            }

            await PopupService!.ToastAsync(it =>
            {
                it.Type = AlertTypes.Success;
                it.Title = "添加成功";
            });
            tagModel.Id = await TagService!.GetLastInsertRowId();
            Tags.Add(tagModel);
            this.StateHasChanged();
        }
        private void NavigateToSearch()
        {
            NavigateService!.NavigateTo("/Search");
        }
        private void NavigateToWrite()
        {
            NavigateService!.NavigateTo("/Write");
        }
        private void SetShowAddTag(bool value)
        {
            if (_showAddTag != value)
            {
                _showAddTag = value;
                if (value)
                {
                    NavigateService!.Action += CloseAddTag;
                }
                else
                {
                    NavigateService!.Action -= CloseAddTag;
                }
            }
        }
        private void CloseAddTag()
        {
            ShowAddTag = false;
            StateHasChanged();
        }
        private string GetWelcomeText()
        {
            int hour = Convert.ToInt16(DateTime.Now.ToString("HH"));
            if(hour >= 6 && hour < 11)
            {
                return "早上好，又是全新的一天";
            }
            else if(hour >= 11 && hour < 13)
            {
                return "中午好，吃午饭了吗";
            }
            else if (hour >= 13 && hour < 18)
            {
                return "下午好，享受美好的下午时光";
            }
            else if (hour >= 18 && hour < 23)
            {
                return "晚上好，该到了休息的时候";
            }
            else if (hour >= 23 || hour < 6)
            {
                return "凌晨好，愿你好梦";
            }
            return "Hello World";
        }
        public void Dispose()
        {
            if(ShowAddTag)
            {
                NavigateService!.Action -= CloseAddTag;
            }
            GC.SuppressFinalize(this);
        }
    }
}
