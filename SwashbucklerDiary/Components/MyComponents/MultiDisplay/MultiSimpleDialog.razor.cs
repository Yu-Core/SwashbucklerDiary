using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Components
{
    public partial class MultiSimpleDialog : DialogComponentBase
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
    }
}
