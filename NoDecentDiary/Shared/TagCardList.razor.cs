using BlazorComponent;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;

namespace NoDecentDiary.Shared
{
    public partial class TagCardList : IDisposable
    {
        [Inject]
        public IPopupService? PopupService { get; set; }
        [Inject]
        public ITagService? TagService { get; set; }
        [Inject]
        public IDiaryTagService? DiaryTagService { get; set; }
        [Inject]
        public INavigateService? NavigateService { get; set; }

        [Parameter]
        [EditorRequired]
        public List<TagModel>? Value { get; set; }
        private int RenameTagId;
        private string? RenameTagName;
        private Action? HandOnOKDeleteTag { get; set; }
        private bool _showDeleteTag;
        private bool ShowDeleteTag
        {
            get => _showDeleteTag;
            set
            {
                _showDeleteTag = value;
                if (!value)
                {
                    HandOnOKDeleteTag = null;
                }
            }
        }
        private bool _showRenameTag;
        private bool ShowRenameTag
        {
            get => _showRenameTag;
            set
            {
                SetShowRenameTag(value);
            }
        }

        public TagCardList()
        {
            Value ??= new List<TagModel>();
        }
        private void HandOnTagRename(TagModel tag)
        {
            RenameTagId = tag.Id;
            RenameTagName = tag.Name;
            StateHasChanged();
            ShowRenameTag = true;
        }

        private void HandOnTagDelete(TagModel tag)
        {
            HandOnOKDeleteTag += async () =>
            {
                ShowDeleteTag = false;
                bool flag = await TagService!.DeleteAsync(tag);
                if (flag)
                {
                    Value!.Remove(tag);
                    await PopupService!.AlertAsync("删除成功", AlertTypes.Success);
                    this.StateHasChanged();
                    await DiaryTagService!.DeleteAsync(it => it.TagId == tag.Id);
                }
                else
                {
                    await PopupService!.AlertAsync("删除失败", AlertTypes.Error);
                }
            };
            ShowDeleteTag = true;
            StateHasChanged();
        }
        private async Task HandOnSaveRenameTag()
        {
            ShowRenameTag = false;
            if (string.IsNullOrWhiteSpace(RenameTagName))
            {
                return;
            }

            if (Value!.Any(it => it.Name == RenameTagName))
            {
                await PopupService!.AlertAsync("标签已存在", AlertTypes.Warning);
                return;
            }

            var tag = Value!.FirstOrDefault(it => it.Id == RenameTagId);
            tag!.Name = RenameTagName;
            bool flag = await TagService!.UpdateAsync(tag);
            if (flag)
            {
                await PopupService!.AlertAsync("修改成功", AlertTypes.Success);
            }
            else
            {
                await PopupService!.AlertAsync("修改失败", AlertTypes.Error);
            }
        }
        private void HandOnTagClick(int id)
        {
            NavigateService!.NavigateTo($"/Tag/{id}");
        }
        private void SetShowRenameTag(bool value)
        {
            if (_showRenameTag != value)
            {
                _showRenameTag = value;
                if (value)
                {
                    NavigateService!.Action += CloseRenameTag;
                }
                else
                {
                    NavigateService!.Action -= CloseRenameTag;
                }
            }
        }
        private void CloseRenameTag()
        {
            ShowRenameTag = false;
            StateHasChanged();
        }
        public void Dispose()
        {
            if (ShowRenameTag)
            {
                NavigateService!.Action -= CloseRenameTag;
            }
            GC.SuppressFinalize(this);
        }
    }
}
