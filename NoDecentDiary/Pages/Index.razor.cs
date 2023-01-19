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
        public INavigateService? NavigateService { get; set; }
        [Inject]
        public NavigationManager? Navigation { get; set; }
        [CascadingParameter]
        public Error? Error { get; set; }
        [Parameter]
        [SupplyParameterFromQuery]
        public string? Type { get; set; }
        private StringNumber tabs = 0;
        private List<DiaryModel> Diaries { get; set; } = new List<DiaryModel>();
        private List<TagModel> Tags { get; set; } = new List<TagModel>();
        private bool showEditTag;
        private int EditTagId;
        private string? EditTagName;
        private bool showAddTag;
        private string? AddTagName;
        private readonly List<string> Types = new()
        {
            "All", "Tags"
        };
        protected override async Task OnInitializedAsync()
        {
            SetTab();
            await UpdateTags();
            await UpdateDiaries();
        }
        private async Task UpdateDiaries()
        {
            var diaryModels = await DiaryService!.QueryAsync();
            Diaries = diaryModels.Take(50).ToList();
        }
        private async Task UpdateTags()
        {
            Tags = await TagService!.QueryAsync();
        }
        private void SetTab()
        {
            tabs = Types.IndexOf(Type!);
        }
        private void HandOnTagRename(TagModel tag)
        {
            EditTagId = tag.Id;
            EditTagName = tag.Name;
            StateHasChanged();
            showEditTag = true;
        }
        private async Task HandOnSaveEditTag()
        {
            showEditTag = false;
            if (string.IsNullOrWhiteSpace(EditTagName))
            {
                return;
            }

            if (Tags.Any(it => it.Name == EditTagName))
            {
                await PopupService!.AlertAsync("标签已存在", AlertTypes.Warning);
                return;
            }

            var tag = Tags.FirstOrDefault(it => it.Id == EditTagId);
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
                await DiaryTagService!.DeleteAsync(it => it.TagId == tag.Id);
            }
            else
            {
                await PopupService!.AlertAsync("删除失败", AlertTypes.Error);
            }
        }
        private void HandOnTagClick(int id)
        {
            NavigateService!.NavigateTo($"/Tag/{id}");
        }
        private async Task HandOnRefreshData(StringNumber value)
        {
            if (value == 0)
            {
                await UpdateDiaries();
            }
            else if (value == 1)
            {
                await UpdateTags();
            }
            var url = Navigation!.GetUriWithQueryParameter("Type", Types[tabs.ToInt32()]);
            Navigation!.NavigateTo(url);
        }
        private async Task HandOnSaveAddTag()
        {
            showAddTag = false;
            if (string.IsNullOrWhiteSpace(AddTagName))
            {
                return;
            }

            if (Tags.Any(it => it.Name == AddTagName))
            {
                await PopupService!.AlertAsync("标签已存在", AlertTypes.Warning);
                return;
            }

            TagModel tagModel = new TagModel()
            {
                Name = AddTagName
            };
            bool flag = await TagService!.AddAsync(tagModel);
            if (!flag)
            {
                await PopupService!.AlertAsync("添加失败", AlertTypes.Error);
                return;
            }

            await PopupService!.AlertAsync("添加成功", AlertTypes.Success);
            tagModel.Id = await TagService!.GetLastInsertRowId();
            Tags.Add(tagModel);
            this.StateHasChanged();
        }
        private void NavigateToSearch()
        {
            NavigateService!.NavigateTo("/Search");
        }
    }
}
