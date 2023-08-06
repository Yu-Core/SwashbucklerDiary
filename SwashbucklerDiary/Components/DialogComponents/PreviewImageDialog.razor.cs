using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class PreviewImageDialog : DialogComponentBase
    {
        [Parameter]
        public string? Src { get; set; }
    }
}
