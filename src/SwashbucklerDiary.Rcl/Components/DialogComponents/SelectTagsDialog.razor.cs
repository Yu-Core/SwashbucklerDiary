using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SelectTagsDialog : DialogComponentBase
    {
        private bool showAddTag;

        private bool showSearch;

        private string? searchText;

        private List<StringNumber> SelectedTagIds = [];

        private IEnumerable<TagModel> internalItems = [];

        [Inject]
        public ITagService TagService { get; set; } = default!;

        [CascadingParameter(Name = "IsDark")]
        public bool Dark { get; set; }

        [Parameter]
        public List<TagModel> Value { get; set; } = [];

        [Parameter]
        public EventCallback<List<TagModel>> ValueChanged { get; set; }

        [Parameter]
        public EventCallback OnSave { get; set; }

        [Parameter]
        public List<TagModel> Items { get; set; } = [];

        [Parameter]
        public EventCallback<List<TagModel>> ItemsChanged { get; set; }

        private string? Color => Dark ? "white" : "grey";

        private async Task BeforeShowContent()
        {
            searchText = string.Empty;
            internalItems = Items;
            await SetSelectedTagIds();
        }

        private async Task HandleOnSave(MouseEventArgs _)
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

            await InternalVisibleChanged(false);
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
                await PopupServiceHelper.Warning(I18n.T("Tag.Repeat.Title"), I18n.T("Tag.Repeat.Content"));
                return;
            }

            TagModel tag = new()
            {
                Name = tagName
            };
            var flag = await TagService.AddAsync(tag);
            if (!flag)
            {
                await PopupServiceHelper.Error(I18n.T("Share.AddFail"));
                return;
            }

            Items.Insert(0, tag);
            StateHasChanged();
        }

        private void Search(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                internalItems = Items;
            }
            else
            {
                internalItems = Items.Where(it => !string.IsNullOrEmpty(it.Name) && (it.Name.Contains(text) || SelectedTagIds.Any(t => t.ToString() == it.Id.ToString())));
            }
        }
    }
}
