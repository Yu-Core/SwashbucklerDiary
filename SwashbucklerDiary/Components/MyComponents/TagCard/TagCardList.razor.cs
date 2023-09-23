using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class TagCardList : MyComponentBase
    {
        private bool ShowDelete;
        private bool ShowRename;
        private bool ShowExport;
        private TagModel SelectedTag = new();
        private List<TagModel> _value = new();
        private List<DiaryModel> ExportDiaries = new();

        [Inject]
        private ITagService TagService { get; set; } = default!;
        [Inject]
        private IPlatformService PlatformService { get; set; } = default!;

        [Parameter]
        public List<TagModel> Value
        {
            get => _value.OrderByDescending(GetDiaryCount).ToList();
            set => _value = value;
        }
        [Parameter]
        public EventCallback<List<TagModel>> ValueChanged { get; set; }
        [Parameter]
        public List<DiaryModel> Diaries { get; set; } = default!;

        public async Task Rename(TagModel tag)
        {
            SelectedTag = tag;
            ShowRename = true;
            await InvokeAsync(StateHasChanged);
        }

        public async Task Delete(TagModel tag)
        {
            SelectedTag = tag;
            ShowDelete = true;
            await InvokeAsync(StateHasChanged);
        }

        public async Task Export(TagModel tag)
        {
            var flag = await CheckPermission();
            if (!flag)
            {
                return;
            }

            var newTag = await TagService.FindIncludesAsync(tag.Id);
            var diaries = newTag.Diaries;
            if (diaries is null ||  !diaries.Any())
            {
                await AlertService.Info(I18n.T("Diary.NoDiary"));
                return;
            }

            ExportDiaries = diaries;
            ShowExport = true;
            await InvokeAsync(StateHasChanged);
        }

        public int GetDiaryCount(TagModel tag)
            => Diaries.Count(d => d.Tags != null && d.Tags.Any(t => t.Id == tag.Id));

        private async Task ConfirmDelete()
        {
            var tag = SelectedTag;
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

            SelectedTag.Name = tagName;
            SelectedTag.UpdateTime = DateTime.Now;
            bool flag = await TagService.UpdateAsync(SelectedTag);
            if (flag)
            {
                await AlertService.Success(I18n.T("Share.EditSuccess"));
            }
            else
            {
                await AlertService.Error(I18n.T("Share.EditFail"));
            }
        }

        private async Task<bool> CheckPermission()
        {
            var writePermission = await PlatformService.TryStorageWritePermission();
            if (!writePermission)
            {
                await AlertService.Info(I18n.T("Permission.OpenStorageWrite"));
                return false;
            }

            var readPermission = await PlatformService.TryStorageReadPermission();
            if (!readPermission)
            {
                await AlertService.Info(I18n.T("Permission.OpenStorageRead"));
                return false;
            }

            return true;
        }
    }
}
