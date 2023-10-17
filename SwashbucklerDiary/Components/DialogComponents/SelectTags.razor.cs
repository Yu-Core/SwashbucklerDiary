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

        private List<StringNumber> SelectedTagIds = new();

        [Inject]
        public ITagService TagService { get; set; } = default!;

        [Parameter]
        public override bool Value
        {
            get => _value;
            set => SetValue(value);
        }

        [Parameter]
        public List<TagModel> Values { get; set; } = default!;

        [Parameter]
        public EventCallback<List<TagModel>> ValuesChanged { get; set; }

        [Parameter]
        public EventCallback OnSave { get; set; }

        [Parameter]
        public List<TagModel> Tags { get; set; } = new();

        [Parameter]
        public EventCallback<List<TagModel>> TagsChanged { get; set; }

        protected virtual async Task HandleOnSave(MouseEventArgs _)
        {
            var TagModels = new List<TagModel>();
            foreach (var item in SelectedTagIds)
            {
                var TagModel = Tags.FirstOrDefault(it => it.Id.ToString() == item.ToString());
                if (TagModel != null)
                {
                    TagModels.Add(TagModel);
                }
            }
            Values = TagModels;

            if (ValuesChanged.HasDelegate)
            {
                await ValuesChanged.InvokeAsync(TagModels);
            }

            await OnSave.InvokeAsync();
        }

        private void SetValue(bool value)
        {
            if (_value == value)
            {
                return;
            }

            if (value)
            {
                SelectedTagIds = new();
                foreach (var item in Values)
                {
                    if (Tags.Any(it => it.Id == item.Id))
                    {
                        SelectedTagIds.Add(item.Id.ToString());
                    }
                }
            }

            _value = value;
            StateHasChanged();
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
            Tags.Insert(0, tag);
            StateHasChanged();
        }
    }
}
