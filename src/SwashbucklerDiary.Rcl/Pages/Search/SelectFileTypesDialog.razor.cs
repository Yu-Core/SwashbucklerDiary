using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class SelectFileTypesDialog : DialogComponentBase
    {
        private List<DynamicListItem<MediaResource>> items = [];

        [Parameter]
        public List<MediaResource> Value { get; set; } = [];

        [Parameter]
        public EventCallback<List<MediaResource>> ValueChanged { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            LoadView();
        }

        private void LoadView()
        {
            items =
            [
                new(this,"Image","mdi-image-outline",UpdateValue,MediaResource.Image),
                new(this,"Audio","mdi-music",UpdateValue,MediaResource.Audio),
                new(this,"Video","mdi-movie-open-outline",UpdateValue,MediaResource.Video),
            ];
        }

        private async Task UpdateValue(MediaResource mediaResource)
        {
            if (!Value.Remove(mediaResource))
            {
                Value.Add(mediaResource);
            }

            Value = [.. Value];
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
        }
    }
}