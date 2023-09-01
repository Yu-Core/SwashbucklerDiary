using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class TagCardList : MyComponentBase
    {
        private bool ShowDelete;
        private bool ShowRename;
        private TagModel SelectedTag = new();
        private List<TagModel> _value = new();

        [Inject]
        public ITagService TagService { get; set; } = default!;

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

        public void Delete(TagModel tag)
        {
            SelectedTag = tag;
            ShowDelete = true;
            InvokeAsync(StateHasChanged);
        }

        public int GetDiaryCount(TagModel tag) => Diaries.Count(d => d.Tags != null && d.Tags.Any(t => t.Id == tag.Id));

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
    }
}
