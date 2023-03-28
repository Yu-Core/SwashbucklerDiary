using BlazorComponent;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class TagCardList : MyComponentBase
    {
        private bool ShowDeleteTag;
        private bool ShowRenameTag;
        private Guid RenameTagId;
        private TagModel SelectedTag = new();

        [Inject]
        public ITagService TagService { get; set; } = default!;

        [Parameter]
        public List<TagModel>? Value { get; set; } = new();
        [Parameter]
        public EventCallback<List<TagModel>> ValueChanged { get; set; }

        private string? RenameTagName { get; set; }

        private void HandleRename(TagModel tag)
        {
            RenameTagId = tag.Id;
            RenameTagName = tag.Name;
            StateHasChanged();
            ShowRenameTag = true;
        }

        private void OpenDeleteDialog(TagModel tag)
        {
            SelectedTag = tag;
            ShowDeleteTag = true;
        }

        private async Task HandleDelete(TagModel tag)
        {
            ShowDeleteTag = false;
            bool flag = await TagService.DeleteAsync(tag);
            if (flag)
            {
                Value!.Remove(tag);
                if(ValueChanged.HasDelegate)
                {
                    await ValueChanged.InvokeAsync(Value);
                }
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
                StateHasChanged();
            }
            else
            {
                await AlertService.Error(I18n.T("Share.DeleteFail"));
            }
        }

        private async Task SaveRenameTag(string tagName)
        {
            ShowRenameTag = false;
            if (string.IsNullOrWhiteSpace(tagName))
            {
                return;
            }

            if (Value!.Any(it => it.Name == tagName))
            {
                await AlertService.Warning(I18n.T("Tag.Repeat.Title"),I18n.T("Tag.Repeat.Content"));
                return;
            }

            var tag = Value!.FirstOrDefault(it => it.Id == RenameTagId);
            tag!.Name = RenameTagName = tagName;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
            bool flag = await TagService.UpdateAsync(tag);
            if (flag)
            {
                await AlertService.Success(I18n.T("Share.EditSuccess"));
            }
            else
            {
                await AlertService.Error(I18n.T("Share.EditFail"));
            }
        }

        private void HandleClick(Guid id)
        {
            NavigateService.NavigateTo($"/tag/{id}");
        }
    }
}
