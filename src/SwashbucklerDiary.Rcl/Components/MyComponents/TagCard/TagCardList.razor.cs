using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class TagCardList : CardListComponentBase<TagModel>, IAsyncDisposable
    {
        private ElementReference elementRef;

        private bool ShowDelete;

        private bool ShowRename;

        private bool ShowExport;

        private readonly TagCardListOptions options = new();

        private List<DiaryModel> ExportDiaries = [];

        private Dictionary<Guid, int> TagsDiaryCount = [];

        [Inject]
        private ITagService TagService { get; set; } = default!;

        [Inject]
        private IntersectJSModule IntersectJSModule { get; set; } = default!;

        [Parameter]
        public EventCallback<List<TagModel>> ValueChanged { get; set; }

        [Parameter]
        public List<DiaryModel> Diaries { get; set; } = [];

        public async ValueTask DisposeAsync()
        {
            base.OnDispose();
            await IntersectJSModule.UnobserveAsync(elementRef);
            GC.SuppressFinalize(this);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            LoadView();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                var handle = DotNetObjectReference.Create(new IntersectInvoker(OnIntersectAsync));
                await IntersectJSModule.ObserverAsync(elementRef, handle);
            }
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            var tagSort = SettingService.Get<string>(Setting.TagSort);
            if (!string.IsNullOrEmpty(tagSort))
            {
                sortItem = tagSort;
            }
        }

        private async Task ConfirmDelete()
        {
            var tag = SelectedItemValue;
            ShowDelete = false;
            bool flag = await TagService.DeleteAsync(tag);
            if (flag)
            {

                var index = _value.FindIndex(it => it.Id == tag.Id);
                if (index < 0)
                {
                    return;
                }
                _value.RemoveAt(index);
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
                StateHasChanged();
            }
            else
            {
                await AlertService.Error(I18n.T("Share.DeleteFail"));
            }
        }

        private async Task ConfirmRename(string tagName)
        {
            ShowRename = false;
            if (string.IsNullOrWhiteSpace(tagName))
            {
                return;
            }

            if (Value.Any(it => it.Name == tagName))
            {
                await AlertService.Warning(I18n.T("Tag.Repeat.Title"), I18n.T("Tag.Repeat.Content"));
                return;
            }

            SelectedItemValue.Name = tagName;
            SelectedItemValue.UpdateTime = DateTime.Now;
            bool flag = await TagService.UpdateAsync(SelectedItemValue, it => new { it.Name, it.UpdateTime });
            if (!flag)
            {
                await AlertService.Error(I18n.T("Share.EditFail"));
            }
        }

        private void LoadView()
        {
            sortOptions = new()
            {
                {"Sort.Count.Desc", it => it.OrderByDescending(CalcDiaryCount) },
                {"Sort.Count.Asc", it => it.OrderBy(CalcDiaryCount) },
                {"Sort.Name.Desc", it => it.OrderByDescending(t => t.Name) },
                {"Sort.Name.Asc", it => it.OrderBy(t => t.Name) },
                {"Sort.Time.Desc", it => it.OrderByDescending(t => t.CreateTime) },
                {"Sort.Time.Asc", it => it.OrderBy(t => t.CreateTime) },
            };

            if (string.IsNullOrEmpty(sortItem))
            {
                sortItem = SortItems.First();
            }

            menuItems =
            [
                new(this, "Share.Rename", "mdi-rename-outline", Rename),
                new(this, "Share.Delete", "mdi-delete-outline", Delete),
                new(this, "Diary.Export", "mdi-export", Export),
                new(this, "Share.Sort", "mdi-sort-variant", Sort),
            ];
        }

        private void Rename()
        {
            ShowRename = true;
        }

        private void Delete()
        {
            ShowDelete = true;
        }

        private async Task Export()
        {
            var newTag = await TagService.FindIncludesAsync(SelectedItemValue.Id);
            var diaries = newTag.Diaries;
            if (diaries is null || diaries.Count == 0)
            {
                await AlertService.Info(I18n.T("Diary.NoDiary"));
                return;
            }

            ExportDiaries = diaries;
            ShowExport = true;
        }

        private async Task SortChanged(string value)
        {
            await SettingService.Set(Setting.TagSort, value);
        }

        private int CalcDiaryCount(TagModel tag)
        {
            if (TagsDiaryCount.TryGetValue(tag.Id, out var count))
            {
                return count;
            }

            return 0;
        }

        private async Task UpdateTagsDiaryCount()
        {
            TagsDiaryCount = await TagService.TagsDiaryCount();
        }

        private async Task OnIntersectAsync(IntersectEventArgs args)
        {
            if (args.IsIntersecting)
            {
                await UpdateTagsDiaryCount();
                await InvokeAsync(StateHasChanged);
                options.NotifyDiariesChanged();
            }
        }
    }
}
