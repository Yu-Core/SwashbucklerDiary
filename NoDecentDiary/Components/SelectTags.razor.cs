using BlazorComponent;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;

namespace NoDecentDiary.Components
{
    public partial class SelectTags : DialogComponentBase
    {
        private bool _value;
        private bool ShowAddTag;
        private string? AddTagName;
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

        private async void SetValue(bool value)
        {
            if (_value != value)
            {
                if (value)
                {
                    Tags = await TagService!.QueryAsync();
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

        private async Task SaveAddTag()
        {
            ShowAddTag = false;
            if (string.IsNullOrWhiteSpace(AddTagName))
            {
                return;
            }

            if (Tags.Any(it => it.Name == AddTagName))
            {
                await PopupService.ToastAsync(it =>
                {
                    it.Type = AlertTypes.Warning;
                    it.Title = I18n!.T("Tag.Repeat.Title");
                    it.Content = I18n!.T("Tag.Repeat.Content");
                });
                return;
            }

            TagModel tagModel = new()
            {
                Name = AddTagName
            };
            AddTagName = string.Empty;
            bool flag = await TagService!.AddAsync(tagModel);
            if (!flag)
            {
                await PopupService.ToastAsync(it =>
                {
                    it.Type = AlertTypes.Error;
                    it.Title = I18n!.T("Share.AddFail");
                });
                return;
            }

            await PopupService.ToastAsync(it =>
            {
                it.Type = AlertTypes.Success;
                it.Title = I18n!.T("Share.AddSuccess");
            });
            tagModel.Id = await TagService!.GetLastInsertRowId();
            Tags.Add(tagModel);
            StateHasChanged();
        }
    }
}
