using BlazorComponent;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Shared
{
    public partial class TagCardList : IDisposable
    {
        [Inject]
        public IPopupService? PopupService { get; set; }
        [Inject]
        public ITagService? TagService { get; set; }
        [Inject]
        public IDiaryTagService? DiaryTagService { get; set;}
        [Inject]
        public INavigateService? NavigateService { get; set; }

        [Parameter]
        [EditorRequired]
        public List<TagModel>? Value { get; set; }
        private int EditTagId;
        private string? EditTagName;
        private bool _showEditTag;
        private bool ShowEditTag
        {
            get => _showEditTag;
            set
            {
                SetShowEditTag(value);
            }
        }

        public TagCardList()
        {
            Value ??= new List<TagModel>();
        }
        private void HandOnTagRename(TagModel tag)
        {
            EditTagId = tag.Id;
            EditTagName = tag.Name;
            StateHasChanged();
            ShowEditTag = true;
        }
        private async Task HandOnTagDelete(TagModel tag)
        {
            var confirmed = await PopupService!.ConfirmAsync(param =>
            {
                param.Title = "删除标签";
                param.TitleStyle = "font-weight:700;";
                param.Content = "请慎重删除";
                param.IconColor = "red";
                param.ActionsStyle = "justify-content: flex-end;";
            });
            if (!confirmed)
            {
                return;
            }
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
        }
        private async Task HandOnSaveEditTag()
        {
            ShowEditTag = false;
            if (string.IsNullOrWhiteSpace(EditTagName))
            {
                return;
            }

            if (Value!.Any(it => it.Name == EditTagName))
            {
                await PopupService!.AlertAsync("标签已存在", AlertTypes.Warning);
                return;
            }

            var tag = Value!.FirstOrDefault(it => it.Id == EditTagId);
            tag!.Name = EditTagName;
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
        private void SetShowEditTag(bool value)
        {
            if (_showEditTag != value)
            {
                _showEditTag = value;
                if (value)
                {
                    NavigateService!.Action += CloseEditTag;
                }
                else
                {
                    NavigateService!.Action -= CloseEditTag;
                }
            }
        }
        private void CloseEditTag()
        {
            ShowEditTag = false;
            StateHasChanged();
        }
        public void Dispose()
        {
            if (ShowEditTag)
            {
                NavigateService!.Action -= CloseEditTag;
            }
            GC.SuppressFinalize(this);
        }
    }
}
