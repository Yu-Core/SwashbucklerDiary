using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SelectTagsDialog : DialogComponentBase
    {
        private bool showAddTag;

        private bool showSearch;

        private string? _searchText;

        private List<StringNumber> SelectedTagIds = [];

        private List<TagModel> internalItems = [];

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

        private void BeforeShowContent()
        {
            _searchText = string.Empty;
            UpdateInternalItems();
            SetSelectedTagIds();
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

        private void SetSelectedTagIds()
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
                await AlertService.Warning(I18n.T("Tag already exists"), I18n.T("Do not add again"));
                return;
            }

            TagModel tag = new()
            {
                Name = tagName
            };
            var flag = await TagService.AddAsync(tag);
            if (!flag)
            {
                await AlertService.Error(I18n.T("Add failed"));
                return;
            }

            Items.Insert(0, tag);
            UpdateInternalItems();
            Items = [.. Items];
            if (ItemsChanged.HasDelegate)
            {
                await ItemsChanged.InvokeAsync(Items);
            }

            StateHasChanged();
        }

        private void UpdateInternalItems()
        {
            Expression<Func<TagModel, bool>> exp = GetExpression();
            internalItems = Items.Where(exp.Compile()).ToList();
        }

        private Expression<Func<TagModel, bool>> GetExpression()
        {
            Expression<Func<TagModel, bool>>? exp = null;
            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                Expression<Func<TagModel, bool>> expSearch
                    = it => !string.IsNullOrEmpty(it.Name) && (it.Name.Contains(_searchText) || SelectedTagIds.Any(t => t.ToString() == it.Id.ToString()));
                exp = exp.And(expSearch);
            }

            if (exp == null)
            {
                return it => true;
            }

            return exp;
        }
    }
}
