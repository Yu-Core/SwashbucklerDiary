using BlazorComponent;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class ImageResourceCardList 
    {
        private bool ShowPreview;

        private string? PreviewImageSrc;

        private List<ResourceModel> _value = default!;

        private List<ResourceModel> InternalValue = new();

        private readonly int LoadCount = 20;

        private bool FirstLoad = true;

        [CascadingParameter(Name = "ScrollElement")]
        public ElementReference ScrollElement { get; set; }

        [Parameter]
        public List<ResourceModel> Value
        {
            get => _value;
            set => SetValue(value);
        }

        private bool ShowLoadMore => InternalValue.Any() && InternalValue.Count < _value.Count;

        private void SetValue(List<ResourceModel> value)
        {
            if(_value != value)
            {
                _value = value;
                InternalValue = new();
                InternalValue = MockRequest();
            }
        }

        public async Task PreviewImage(string src)
        {
            PreviewImageSrc = src;
            ShowPreview = true;
            await InvokeAsync(StateHasChanged);
        }

        private void OnLoad(InfiniteScrollLoadEventArgs args)
        {
            if (FirstLoad)
            {
                FirstLoad = false;
                return;
            }

            var append = MockRequest();

            InternalValue.AddRange(append);
        }

        private List<ResourceModel> MockRequest()
        {
            return Value.Skip(InternalValue.Count).Take(LoadCount).ToList();
        }
    }
}
