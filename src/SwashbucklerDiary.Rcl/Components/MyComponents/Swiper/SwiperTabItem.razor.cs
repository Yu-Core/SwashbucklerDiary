using BlazorComponent;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SwiperTabItem : IDisposable
    {
        [CascadingParameter]
        public SwiperTabItems? TabItems { get; set; }

        [Parameter]
        public string? Id { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public RenderFragment? FixContent { get; set; }

        public ElementReference Ref { get; set; }

        public StringNumber? Value { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            TabItems?.RegisterTabItem(this);
        }

        public void Dispose()
        {
            TabItems?.UnregisterTabItem(this);
            GC.SuppressFinalize(this);
        }
    }
}
