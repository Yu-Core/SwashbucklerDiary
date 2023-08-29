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
            get => _value.OrderByDescending(it => it.Diaries is null ? 0 : it.Diaries.Count).ToList();
            set => _value = value;
        }
        [Parameter]
        public EventCallback<List<TagModel>> ValueChanged { get; set; }

        private void OpenRenameDialog(TagModel tag)
        {
            SelectedTag = tag;
            StateHasChanged();
            ShowRename = true;
        }

        private void OpenDeleteDialog(TagModel tag)
        {
            SelectedTag = tag;
            ShowDelete = true;
        }

        private async Task HandleDelete(TagModel tag)
        {
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

        private async Task HandleRename(string tagName)
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

        private void HandleClick(TagModel tag)
        {
            NavigateService.NavigateTo($"/tag/{tag.Id}");
        }
    }
}
