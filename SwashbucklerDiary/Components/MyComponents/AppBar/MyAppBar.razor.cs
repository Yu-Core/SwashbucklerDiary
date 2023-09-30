using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace SwashbucklerDiary.Components
{
    public partial class MyAppBar
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }
    }
}
