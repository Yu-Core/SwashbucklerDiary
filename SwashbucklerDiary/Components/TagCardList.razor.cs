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
        private int RenameTagId;
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
                    await PopupService.ToastAsync(it =>
                    {
                        it.Type = AlertTypes.Success;
                        it.Title = I18n.T("Share.DeleteSuccess");
                    });
                    StateHasChanged();
                }
                else
                {
                    await PopupService.ToastAsync(it =>
                    {
                        it.Type = AlertTypes.Error;
                        it.Title = I18n.T("Share.DeleteFail");
                    });
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
                await PopupService.ToastAsync(it =>
                {
                    it.Type = AlertTypes.Warning;
                    it.Title = I18n.T("Tag.Repeat.Title");
                    it.Content = I18n.T("Tag.Repeat.Content");
                });
                return;
            }

            var tag = Value!.FirstOrDefault(it => it.Id == RenameTagId);
            tag!.Name = RenameTagName = tagName;
            bool flag = await TagService.UpdateAsync(tag);
            if (flag)
            {
                await PopupService.ToastAsync(it =>
                {
                    it.Type = AlertTypes.Success;
                    it.Title = I18n.T("Share.EditSuccess");
                });
            }
            else
            {
                await PopupService.ToastAsync(it =>
                {
                    it.Type = AlertTypes.Error;
                    it.Title = I18n.T("Share.EditFail");
                });
            }
        }

        private void OnClick(int id)
        {
            NavigateService.NavigateTo($"/tag/{id}");
        }
    }
}
