using BlazorComponent;
using BlazorComponent.I18n;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;
using NoDecentDiary.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Pages
{
    public partial class Index
    {
        [Inject]
        public I18n? I18n { get; set; }
        [Inject]
        public IDiaryService? DiaryService { get; set; }
        [Inject]
        public ITagService? TagService { get; set; }
        [Inject]
        public IDiaryTagService? DiaryTagService { get; set; }
        [Inject]
        public IPopupService? PopupService { get; set; }
        [Inject]
        public NavigationManager? NavigationManager { get; set; }
        [Parameter]
        public string? Tab { get; set; }
        [CascadingParameter]
        public Error? Error { get; set; }
        private StringNumber tabs = 0;
        private List<DiaryModel> Diaries { get; set; } = new List<DiaryModel>();
        private List<TagModel> Tags { get; set; } = new List<TagModel>();
        private bool showEditTag;
        private int EditTagId;
        private string? EditTagName;
        protected override async Task OnInitializedAsync()
        {
            await UpdateDiaries();
            await UpdateTags();
            if(Tab == "tags")
            {
                tabs = 1;
            }
        }
        private async Task UpdateDiaries()
        {
            var diaryModels = await DiaryService!.QueryAsync();
            Diaries = diaryModels.Take(50)
                .OrderByDescending(it => it.CreateTime)
                .ToList();
        }
        private async Task UpdateTags()
        {
            Tags = await TagService!.QueryAsync();
        }
        private void HandOnTagRename(TagModel tag)
        {
            EditTagId= tag.Id;
            EditTagName= tag.Name;
            StateHasChanged();
            showEditTag = true;
        }
        private async Task HandOnSaveEditTag()
        {
            if (string.IsNullOrWhiteSpace(EditTagName))
            {
                return;
            }

            if (Tags.Any(it => it.Name == EditTagName))
            {
                await PopupService!.AlertAsync("标签已存在", AlertTypes.Warning);
                return;
            }

            var tag = Tags.FirstOrDefault(it=>it.Id == EditTagId);
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
            showEditTag = false;
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
                Tags.Remove(tag);
                await PopupService!.AlertAsync("删除成功", AlertTypes.Success);
                this.StateHasChanged();
                await DiaryTagService!.DeleteAsync(it=>it.TagId == tag.Id);
            }
            else
            {
                await PopupService!.AlertAsync("删除失败", AlertTypes.Error);
            }
        }
        private void HandOnTagClick()
        {
            NavigationManager!.NavigateTo("/Write");
        }
    }
}
