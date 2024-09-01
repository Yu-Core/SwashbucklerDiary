using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class TagCardList : CardListComponentBase<TagModel>
    {
        private bool ShowDelete;

        private bool ShowRename;

        private bool ShowExport;

        private readonly TagCardListOptions options = new();

        private List<DiaryModel> ExportDiaries = [];

        private Dictionary<Guid, int> TagsDiaryCount = [];

        [Inject]
        private ITagService TagService { get; set; } = default!;

        [Parameter]
        public List<DiaryModel> Diaries
        {
            get => GetValue<List<DiaryModel>>() ?? [];
            set => SetValue(value);
        }

        [Parameter]
        public EventCallback<List<DiaryModel>> DiariesChanged { get; set; }


        protected override void OnInitialized()
        {
            base.OnInitialized();
            LoadView();
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            var tagSort = SettingService.Get(s => s.TagSort);
            if (!string.IsNullOrEmpty(tagSort))
            {
                SortItem = tagSort;
            }
        }

        protected override void RegisterWatchers(PropertyWatcher watcher)
        {
            base.RegisterWatchers(watcher);

            watcher.Watch<List<DiaryModel>>(nameof(Diaries), async () =>
            {
                TagsDiaryCount = await TagService.TagsDiaryCount();
                UpdateInternalValue();
                options.NotifyDiariesChanged();
            }, immediate: true);
        }

        private async Task ConfirmDelete()
        {
            ShowDelete = false;
            bool flag = await TagService.DeleteAsync(SelectedItem);
            if (flag)
            {
                if (RemoveSelectedItem())
                {
                    await PopupServiceHelper.Success(I18n.T("Share.DeleteSuccess"));
                    StateHasChanged();
                }
            }
            else
            {
                await PopupServiceHelper.Error(I18n.T("Share.DeleteFail"));
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
                await PopupServiceHelper.Warning(I18n.T("Tag.Repeat.Title"), I18n.T("Tag.Repeat.Content"));
                return;
            }

            SelectedItem.Name = tagName;
            SelectedItem.UpdateTime = DateTime.Now;
            bool flag = await TagService.UpdateAsync(SelectedItem, it => new { it.Name, it.UpdateTime });
            if (!flag)
            {
                await PopupServiceHelper.Error(I18n.T("Share.EditFail"));
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

            if (string.IsNullOrEmpty(SortItem))
            {
                SortItem = SortItems.First();
            }

            menuItems =
            [
                new(this, "Share.Rename", "mdi-rename-outline", Rename),
                new(this, "Share.Delete", "mdi-delete-outline", Delete),
                new(this, "Diary.Export", "mdi-export", Export),
                new(this, "Share.Sort", "mdi-sort-variant", OpenSortDialog),
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
            var newTag = await TagService.FindIncludesAsync(SelectedItem.Id);
            var diaries = newTag.Diaries;
            if (diaries is null || diaries.Count == 0)
            {
                await PopupServiceHelper.Info(I18n.T("Diary.NoDiary"));
                return;
            }

            ExportDiaries = diaries;
            ShowExport = true;
        }

        private async Task SortChanged(string value)
        {
            await SettingService.SetAsync(s => s.TagSort, value);
        }

        private int CalcDiaryCount(TagModel tag)
        {
            if (TagsDiaryCount.TryGetValue(tag.Id, out var count))
            {
                return count;
            }

            return 0;
        }
    }
}
