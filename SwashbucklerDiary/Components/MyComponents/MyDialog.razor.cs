using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Components
{
    public partial class MyDialog : DialogComponentBase
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
    }
}
