using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class ImageResourceCard : ITempCustomSchemeAssist
    {
        [Inject]
        public IJSRuntime JS { get; set; } = default!;

        [Parameter]
        public ResourceModel Value { get; set; } = default!;
        [CascadingParameter]
        protected ImageResourceCardList ImageResourceCardList { get; set; } = default!;

        private string Src => this.CustomSchemeRender(Value.ResourceUri!);

        private async Task PreviewImage()
        {
            await ImageResourceCardList.PreviewImage(Src);
        }
    }
}
