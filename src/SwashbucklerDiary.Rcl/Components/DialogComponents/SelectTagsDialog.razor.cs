using BlazorComponent;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SelectTagsDialog : ShowContentDialogComponentBase
    {
        private bool showAddTag;

        private List<StringNumber> SelectedTagIds = [];

        [Inject]
        public ITagService TagService { get; set; } = default!;

        [Parameter]
        public List<TagModel> Value { get; set; } = default!;

        [Parameter]
        public EventCallback<List<TagModel>> ValueChanged { get; set; }

        [Parameter]
        public EventCallback OnSave { get; set; }

        [Parameter]
        public List<TagModel> Items { get; set; } = [];

        [Parameter]
        public EventCallback<List<TagModel>> ItemsChanged { get; set; }

        protected override async Task BeforeShowContent()
        {
            await base.BeforeShowContent();
            await SetSelectedTagIds();
        }

        protected virtual async Task HandleOnSave(MouseEventArgs _)
        {
            var tagModels = new List<TagModel>();
            foreach (var item in SelectedTagIds)
            {
                var tagModel = Items.FirstOrDefault(it => it.Id.ToString() == item.ToString());
                if (tagModel != null)
                {
                    tagModels.Add(tagModel);
                }
            }
            Value = tagModels;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(tagModels);
            }

            await OnSave.InvokeAsync();
        }

        private Task SetSelectedTagIds()
        {
            List<StringNumber> selectedTagIds = [];
            foreach (var item in Value)
            {
                if (Items.Any(it => it.Id == item.Id))
                {
                    selectedTagIds.Add(item.Id.ToString());
                }
            }

            SelectedTagIds = selectedTagIds;
            return Task.CompletedTask;
        }

        private async Task SaveAddTag(string tagName)
        {
            showAddTag = false;
            if (string.IsNullOrWhiteSpace(tagName))
            {
                return;
            }

            if (Items.Any(it => it.Name == tagName))
            {
                await AlertService.Warning(I18n.T("Tag.Repeat.Title"), I18n.T("Tag.Repeat.Content"));
                return;
            }

            TagModel tag = new()
            {
                Name = tagName
            };
            var flag = await TagService.AddAsync(tag);
            if (!flag)
            {
                await AlertService.Error(I18n.T("Share.AddFail"));
                return;
            }

            await AlertService.Success(I18n.T("Share.AddSuccess"));
            Items.Insert(0, tag);
            StateHasChanged();
        }
    }
}
