using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class ImageResourceCardList 
    {
        private bool ShowPreview;
        private string? PreviewImageSrc;

        [Parameter]
        public List<ResourceModel> Value { get; set; } = new();

        public async Task PreviewImage(string src)
        {
            PreviewImageSrc = src;
            ShowPreview = true;
            await InvokeAsync(StateHasChanged);
        }
    }
}
