using BlazorComponent;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class ImageResourceCardList 
    {
        private bool showPreview;

        private string? previewImageSrc;

        private List<ResourceModel> _value = default!;

        private List<ResourceModel> internalValue = [];

        private readonly int loadCount = 20;

        private bool firstLoad = true;

        [CascadingParameter(Name = "ScrollElement")]
        public ElementReference ScrollElement { get; set; }

        [Parameter]
        public List<ResourceModel> Value
        {
            get => _value;
            set => SetValue(value);
        }

        private bool ShowLoadMore => internalValue.Count != 0 && internalValue.Count < _value.Count;

        private void SetValue(List<ResourceModel> value)
        {
            if(_value != value)
            {
                _value = value;
                internalValue = new();
                internalValue = MockRequest();
            }
        }

        public async Task PreviewImage(string src)
        {
            previewImageSrc = src;
            showPreview = true;
            await InvokeAsync(StateHasChanged);
        }

        private void OnLoad(InfiniteScrollLoadEventArgs args)
        {
            if (firstLoad)
            {
                firstLoad = false;
                return;
            }

            var append = MockRequest();

            internalValue.AddRange(append);
        }

        private List<ResourceModel> MockRequest()
        {
            return Value.Skip(internalValue.Count).Take(loadCount).ToList();
        }
    }
}
