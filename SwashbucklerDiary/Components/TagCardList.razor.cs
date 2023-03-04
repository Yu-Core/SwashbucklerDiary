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
        private Action? OnDelete { get; set; }

        [Inject]
        public ITagService TagService { get; set; } = default!;

        [Parameter]
        public List<TagModel>? Value { get; set; } = new();

        private void OnRename(TagModel tag)
        {
            RenameTagId = tag.Id;
            RenameTagName = tag.Name;
            StateHasChanged();
            ShowRenameTag = true;
        }

        private string? RenameTagName { get; set; }

        private void OpenDeleteDialog(TagModel tag)
        {
            OnDelete = null;
            OnDelete += async () =>
            {
                ShowDeleteTag = false;
                bool flag = await TagService.DeleteAsync(tag);
                if (flag)
                {
                    Value!.Remove(tag);
                    await AlertService.Success(I18n.T("Share.DeleteSuccess"));
                    StateHasChanged();
                }
                else
                {
                    await AlertService.Error(I18n.T("Share.DeleteFail"));
                }
            };
            ShowDeleteTag = true;
            StateHasChanged();
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

        private void OnClick(Guid id)
        {
            NavigateService.NavigateTo($"/tag/{id}");
        }
    }
}
