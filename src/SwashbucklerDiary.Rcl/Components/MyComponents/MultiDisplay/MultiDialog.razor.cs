using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MultiDialog : DialogComponentBase
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
    }
}
