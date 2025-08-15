using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class TagCardList : CardListComponentBase<TagModel>
    {
        private bool showDelete;

        private bool showRename;

        private bool showExport;

        private readonly TagCardListOptions options = new();

        private List<DiaryModel> exportDiaries = [];

        private Dictionary<Guid, int> tagsDiaryCount = [];

        [Inject]
        private ITagService TagService { get; set; } = default!;

        [Parameter]
        public List<DiaryModel> Diaries
        {
            get => GetValue<List<DiaryModel>>() ?? [];
            set => SetValue(value);
        }

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
                tagsDiaryCount = await TagService.TagsDiaryCount();
                UpdateInternalValue();
                options.NotifyDiariesChanged();
            }, immediate: true);
        }

        private async Task ConfirmDelete()
        {
            showDelete = false;
            bool flag = await TagService.DeleteAsync(SelectedItem);
            if (flag)
            {
                if (RemoveSelectedItem())
                {
                    await AlertService.SuccessAsync(I18n.T("Delete successfully"));
                    StateHasChanged();
                }
            }
            else
            {
                await AlertService.ErrorAsync(I18n.T("Delete failed"));
            }
        }

        private async Task ConfirmRename(string tagName)
        {
            showRename = false;
            if (string.IsNullOrWhiteSpace(tagName))
            {
                return;
            }

            if (Value.Any(it => it.Name == tagName))
            {
                await AlertService.WarningAsync(I18n.T("Tag already exists"), I18n.T("Do not add again"));
                return;
            }

            SelectedItem.Name = tagName;
            SelectedItem.UpdateTime = DateTime.Now;
            bool flag = await TagService.UpdateAsync(SelectedItem, it => new { it.Name, it.UpdateTime });
            if (!flag)
            {
                await AlertService.ErrorAsync(I18n.T("Change failed"));
            }
        }

        private void LoadView()
        {
            sortOptions = new()
            {
                {"Count - Reverse order", it => it.OrderByDescending(CalcDiaryCount) },
                {"Count - Positive order", it => it.OrderBy(CalcDiaryCount) },
                {"Name - Reverse order", it => it.OrderByDescending(t => t.Name) },
                {"Name - Positive order", it => it.OrderBy(t => t.Name) },
                {"Create Time - Reverse order", it => it.OrderByDescending(t => t.CreateTime) },
                {"Create Time - Positive order", it => it.OrderBy(t => t.CreateTime) },
                {"Update Time - Reverse order",it => it.OrderByDescending(t => t.UpdateTime) },
                {"Update Time - Positive order",it => it.OrderBy(t => t.UpdateTime) },
            };

            if (string.IsNullOrEmpty(SortItem))
            {
                SortItem = SortItems.First();
            }

            menuItems =
            [
                new(this, "Rename", "edit_square", Rename),
                new(this, "Delete", "mdi:mdi-delete-outline", Delete),
                new(this, "Export", "mdi:mdi-export", Export),
                new(this, "Sort", "sort", OpenSortDialog),
            ];
        }

        private void Rename()
        {
            showRename = true;
        }

        private void Delete()
        {
            showDelete = true;
        }

        private async Task Export()
        {
            var newTag = await TagService.FindIncludesAsync(SelectedItem.Id);
            var diaries = newTag.Diaries;
            if (diaries is null || diaries.Count == 0)
            {
                await AlertService.InfoAsync(I18n.T("No diary"));
                return;
            }

            exportDiaries = diaries;
            showExport = true;
        }

        private async Task SortChanged(string value)
        {
            await SettingService.SetAsync(s => s.TagSort, value);
        }

        private int CalcDiaryCount(TagModel tag)
        {
            if (tagsDiaryCount.TryGetValue(tag.Id, out var count))
            {
                return count;
            }

            return 0;
        }
    }
}
