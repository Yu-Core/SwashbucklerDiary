using BlazorComponent;
using Microsoft.AspNetCore.Components;
using OneOf;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class ImageResourceCardList 
    {
        private bool ShowPreview;
        private string? PreviewImageSrc;
        private List<ResourceModel> _value = default!;
        private List<ResourceModel> InternalValue = new();
        private int loadCount = 20;

        [Parameter]
        public List<ResourceModel> Value
        {
            get => _value;
            set => SetValue(value);
        }
        [Parameter]
        public OneOf<ElementReference, string>? ScrollParent { get; set; }

        private void SetValue(List<ResourceModel> value)
        {
            bool first = _value is null;
            _value = value;
            if (!first)
            {
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
            var append = MockRequest();

            InternalValue.AddRange(append);

            args.Status = InternalValue.Count == Value.Count ? InfiniteScrollLoadStatus.Empty : InfiniteScrollLoadStatus.Ok;
        }

        private List<ResourceModel> MockRequest()
        {
            return Value.Skip(InternalValue.Count).Take(loadCount).ToList();
        }
    }
}
