using BlazorComponent;
using BlazorComponent.I18n;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;
using NoDecentDiary.Shared;

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
                await PopupService!.AlertAsync("标签已存在", AlertTypes.Warning);
                return;
            }

            TagModel tagModel = new TagModel()
            {
                Name = AddTagName
            };
            bool flag = await TagService!.AddAsync(tagModel);
            if (!flag)
            {
                await PopupService!.AlertAsync("添加失败", AlertTypes.Error);
                return;
            }

            await PopupService!.AlertAsync("添加成功", AlertTypes.Success);
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
