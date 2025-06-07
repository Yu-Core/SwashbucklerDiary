using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SelectFileTypesDialog : DialogComponentBase
    {
        private List<DynamicListItem<MediaResource>> items = [];

        private List<MediaResource> internalValue = [];

        [Parameter]
        public List<MediaResource> Value { get; set; } = [];

        [Parameter]
        public EventCallback<List<MediaResource>> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<List<MediaResource>> OnOK { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            LoadView();
        }

        private void LoadView()
        {
            items =
            [
                new(this,"Image","image",UpdateInternalValue,MediaResource.Image),
                new(this,"Audio","mdi:mdi-music",UpdateInternalValue,MediaResource.Audio),
                new(this,"Video","mdi:mdi-movie-open-outline",UpdateInternalValue,MediaResource.Video),
            ];
        }

        private void UpdateInternalValue(MediaResource mediaResource)
        {
            if (!internalValue.Remove(mediaResource))
            {
                internalValue.Add(mediaResource);
            }
        }

        private void HandleBeforeShowContent()
        {
            internalValue = [.. Value];
        }

        private async Task HandleOnOK()
        {
            if (OnOK.HasDelegate)
            {
                await OnOK.InvokeAsync(internalValue);
            }
            else
            {
                await InternalVisibleChanged(false);

                Value = [.. internalValue];
                if (ValueChanged.HasDelegate)
                {
                    await ValueChanged.InvokeAsync(Value);
                }
            }
        }
    }
}