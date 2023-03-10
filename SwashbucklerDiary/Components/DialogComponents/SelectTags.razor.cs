using BlazorComponent;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class SelectTags : DialogComponentBase
    {
        private bool _value;
        private bool ShowAddTag;
        private List<StringNumber> SelectedTagIndices = new();
        private List<TagModel> Tags = new();

        [Inject]
        public ITagService TagService { get; set; } = default!;

        [Parameter]
        public override bool Value
        {
            get => _value;
            set => SetValue(value);
        }
        [Parameter]
        public List<TagModel> Values { get; set; } = new List<TagModel>();
        [Parameter]
        public EventCallback<List<TagModel>> ValuesChanged { get; set; }
        [Parameter]
        public EventCallback OnSave { get; set; }

        protected virtual async Task HandleOnSave(MouseEventArgs _)
        {
            var TagModels = new List<TagModel>();
            foreach (var item in SelectedTagIndices)
            {
                var TagModel = Tags[item.ToInt32()];
                TagModels.Add(TagModel);
            }
            Values = TagModels;

            if (ValuesChanged.HasDelegate)
            {
                await ValuesChanged.InvokeAsync(TagModels);
            }

            await OnSave.InvokeAsync();
        }

        private string? AddTagName { get; set; }

        private async void SetValue(bool value)
        {
            if (_value != value)
            {
                if (value)
                {
                    Tags = await TagService.QueryAsync();
                    SelectedTagIndices.Clear();
                    foreach (var item in Values)
                    {
                        int index = Tags.FindIndex(it => it.Id == item.Id); ;
                        if (index > -1)
                        {
                            SelectedTagIndices.Add(index);
                        }
                    }
                }
                _value = value;
                StateHasChanged();
            }
        }

        private async Task SaveAddTag(string tagName)
        {
            ShowAddTag = false;
            if (string.IsNullOrWhiteSpace(tagName))
            {
                return;
            }

            if (Tags.Any(it => it.Name == tagName))
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
            Tags.Add(tag);
            StateHasChanged();
        }
    }
}
