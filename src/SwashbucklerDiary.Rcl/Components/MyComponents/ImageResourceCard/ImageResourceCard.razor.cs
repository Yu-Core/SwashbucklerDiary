using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class ImageResourceCard
    {
        [Inject]
        public IJSRuntime JS { get; set; } = default!;

        [Parameter]
        public ResourceModel Value { get; set; } = default!;

        [CascadingParameter]
        protected ImageResourceCardList ImageResourceCardList { get; set; } = default!;

        private string Src => Value.ResourceUri!;

        private Task PreviewImage() => ImageResourceCardList.PreviewImage(Src);
    }
}
