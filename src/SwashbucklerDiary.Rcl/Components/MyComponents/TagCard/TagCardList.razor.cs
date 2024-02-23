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

        private List<DiaryModel> ExportDiaries = new();

        [Inject]
        private ITagService TagService { get; set; } = default!;

        [Parameter]
        public EventCallback<List<TagModel>> ValueChanged { get; set; }

        [Parameter]
        public List<DiaryModel> Diaries { get; set; } = new();

        public int GetDiaryCount(TagModel tag)
            => Diaries.Count(d => d.Tags != null && d.Tags.Any(t => t.Id == tag.Id));

        protected override void OnInitialized()
        {
            base.OnInitialized();
            LoadView();
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
            if (flag)
            {
                await AlertService.Success(I18n.T("Share.EditSuccess"));
            }
            else
            {
                await AlertService.Error(I18n.T("Share.EditFail"));
            }
        }

        private void LoadView()
        {
            sortOptions = new()
            {
                {"Sort.DiaryCount.Desc", it => it.OrderByDescending(GetDiaryCount) },
                {"Sort.DiaryCount.Asc", it => it.OrderBy(GetDiaryCount) },
                {"Sort.CreateTime.Desc", it => it.OrderByDescending(t => t.CreateTime) },
                {"Sort.CreateTime.Asc", it => it.OrderBy(t => t.CreateTime) },
            };
            sortItem = SortItems.First();
            menuItems = new()
            {
                new(this, "Share.Rename", "mdi-rename-outline", Rename),
                new(this, "Share.Delete", "mdi-delete-outline", Delete),
                new(this, "Diary.Export", "mdi-export", Export),
                new(this, "Share.Sort", "mdi-sort-variant", Sort),
            };
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
    }
}
