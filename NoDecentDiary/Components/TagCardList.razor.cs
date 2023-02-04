using BlazorComponent;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;

namespace NoDecentDiary.Components
{
    public partial class TagCardList : MyComponentBase, IDisposable
    {
        private int RenameTagId;
        private string? RenameTagName;
        private Action? OnDelete{ get; set; }
        private bool _showDeleteTag;
        private bool _showRenameTag;

        [Inject]
        public ITagService TagService { get; set; } = default!;
        [Inject]
        public IDiaryTagService DiaryTagService { get; set; } = default!;

        [Parameter]
        public List<TagModel>? Value { get; set; } = new();

        public void Dispose()
        {
            if (ShowRenameTag)
            {
                NavigateService.Action -= CloseRenameTag;
            }
            GC.SuppressFinalize(this);
        }

        private bool ShowDeleteTag
        {
            get => _showDeleteTag;
            set
            {
                _showDeleteTag = value;
                if (!value)
                {
                    OnDelete = null;
                }
            }
        }
        private bool ShowRenameTag
        {
            get => _showRenameTag;
            set
            {
                SetShowRenameTag(value);
            }
        }

        private void OnRename(TagModel tag)
        {
            RenameTagId = tag.Id;
            RenameTagName = tag.Name;
            StateHasChanged();
            ShowRenameTag = true;
        }

        private void OpenDeleteDialog(TagModel tag)
        {
            OnDelete += async () =>
            {
                ShowDeleteTag = false;
                bool flag = await TagService!.DeleteAsync(tag);
                if (flag)
                {
                    Value!.Remove(tag);
                    await PopupService.ToastAsync(it =>
                    {
                        it.Type = AlertTypes.Success;
                        it.Title = I18n!.T("Share.DeleteSuccess");
                    });
                    StateHasChanged();
                    await DiaryTagService!.DeleteAsync(it => it.TagId == tag.Id);
                }
                else
                {
                    await PopupService.ToastAsync(it =>
                    {
                        it.Type = AlertTypes.Error;
                        it.Title = I18n!.T("Share.DeleteFail");
                    });
                }
            };
            ShowDeleteTag = true;
            StateHasChanged();
        }

        private async Task SaveRenameTag()
        {
            ShowRenameTag = false;
            if (string.IsNullOrWhiteSpace(RenameTagName))
            {
                return;
            }

            if (Value!.Any(it => it.Name == RenameTagName))
            {
                await PopupService.ToastAsync(it =>
                {
                    it.Type = AlertTypes.Warning;
                    it.Title = I18n!.T("Tag.Repeat.Title");
                    it.Content = I18n!.T("Tag.Repeat.Content");
                });
                return;
            }

            var tag = Value!.FirstOrDefault(it => it.Id == RenameTagId);
            tag!.Name = RenameTagName;
            bool flag = await TagService!.UpdateAsync(tag);
            if (flag)
            {
                await PopupService.ToastAsync(it =>
                {
                    it.Type = AlertTypes.Success;
                    it.Title = I18n!.T("Share.EditSuccess");
                });
            }
            else
            {
                await PopupService.ToastAsync(it =>
                {
                    it.Type = AlertTypes.Error;
                    it.Title = I18n!.T("Share.EditFail");
                });
            }
        }

        private void OnClick(int id)
        {
            NavigateService.NavigateTo($"/tag/{id}");
        }

        private void SetShowRenameTag(bool value)
        {
            if (_showRenameTag != value)
            {
                _showRenameTag = value;
                if (value)
                {
                    NavigateService.Action += CloseRenameTag;
                }
                else
                {
                    NavigateService.Action -= CloseRenameTag;
                }
            }
        }

        private void CloseRenameTag()
        {
            ShowRenameTag = false;
            StateHasChanged();
        }
        
    }
}
