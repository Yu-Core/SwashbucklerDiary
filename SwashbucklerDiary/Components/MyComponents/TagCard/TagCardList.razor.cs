using BlazorComponent;
using Microsoft.AspNetCore.Components;
using OneOf;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class TagCardList : MyComponentBase
    {
        private bool ShowDelete;
        private bool ShowRename;
        private TagModel SelectedTag = new();
        private List<TagModel> _value = default!;
        private List<TagModel> InternalValue = new();
        private int loadCount = 50;

        [Inject]
        public ITagService TagService { get; set; } = default!;

        [Parameter]
        public List<TagModel> Value
        {
            get => _value;
            set => SetValue(value);
        }
        [Parameter]
        public EventCallback<List<TagModel>> ValueChanged { get; set; }
        [Parameter]
        public OneOf<ElementReference, string>? ScrollParent { get; set; }

        private void SetValue(List<TagModel> value)
        {
            bool first = _value is null;
            _value = value;
            if (!first)
            {
                InternalValue = new();
                InternalValue = MockRequest();
            }
        }

        private void OpenRenameDialog(TagModel tag)
        {
            SelectedTag = tag;
            StateHasChanged();
            ShowRename = true;
        }

        private void OpenDeleteDialog(TagModel tag)
        {
            SelectedTag = tag;
            ShowDelete = true;
        }

        private async Task HandleDelete(TagModel tag)
        {
            ShowDelete = false;
            bool flag = await TagService.DeleteAsync(tag);
            if (flag)
            {
                Value!.Remove(tag);
                if(ValueChanged.HasDelegate)
                {
                    await ValueChanged.InvokeAsync(Value);
                }
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
                StateHasChanged();
            }
            else
            {
                await AlertService.Error(I18n.T("Share.DeleteFail"));
            }
        }

        private async Task HandleRename(string tagName)
        {
            ShowRename = false;
            if (string.IsNullOrWhiteSpace(tagName))
            {
                return;
            }

            if (Value!.Any(it => it.Name == tagName))
            {
                await AlertService.Warning(I18n.T("Tag.Repeat.Title"),I18n.T("Tag.Repeat.Content"));
                return;
            }

            SelectedTag.Name = tagName;
            bool flag = await TagService.UpdateAsync(SelectedTag);
            if (flag)
            {
                await AlertService.Success(I18n.T("Share.EditSuccess"));
            }
            else
            {
                await AlertService.Error(I18n.T("Share.EditFail"));
            }
        }

        private void HandleClick(TagModel tag)
        {
            NavigateService.NavigateTo($"/tag/{tag.Id}");
        }

        private void OnLoad(InfiniteScrollLoadEventArgs args)
        {
            var append = MockRequest();

            InternalValue.AddRange(append);

            args.Status = InternalValue.Count == Value.Count ? InfiniteScrollLoadStatus.Empty : InfiniteScrollLoadStatus.Ok;
        }

        private List<TagModel> MockRequest()
        {
            return Value.Skip(InternalValue.Count).Take(loadCount).ToList();
        }
    }
}
