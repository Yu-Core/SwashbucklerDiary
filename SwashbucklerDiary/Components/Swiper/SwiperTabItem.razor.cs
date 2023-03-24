using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Components
{
    public partial class SwiperTabItem
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        [Parameter]
        public RenderFragment? FixContent { get; set; }
    }
}
