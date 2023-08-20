using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Components
{
    public partial class PreviewImageDialog : DialogComponentBase
    {
        [Parameter]
        public string? Src { get; set; }
    }
}
