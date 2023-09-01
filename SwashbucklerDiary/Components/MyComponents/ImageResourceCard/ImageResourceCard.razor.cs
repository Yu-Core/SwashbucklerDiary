using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Models;
using SwashbucklerDiary.Utilities;

namespace SwashbucklerDiary.Components
{
    public partial class ImageResourceCard
    {
        [Inject]
        public IJSRuntime JS { get; set; } = default!;

        [Parameter]
        public ResourceModel Value { get; set; } = default!;
        [CascadingParameter]
        protected ImageResourceCardList ImageResourceCardList { get; set; } = default!;

        private string Src => StaticCustomScheme.CustomSchemeRender(Value.ResourceUri!);

        private Task PreviewImage() => ImageResourceCardList.PreviewImage(Src);
    }
}
