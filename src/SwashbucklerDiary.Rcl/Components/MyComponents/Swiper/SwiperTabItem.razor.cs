using BlazorComponent;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SwiperTabItem : IDisposable
    {
        private bool activated;

        private bool rendered;

        [CascadingParameter]
        public SwiperTabItems? TabItems { get; set; }

        [Parameter]
        public string? Id { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        public ElementReference Ref { get; set; }

        public StringNumber? Value { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            TabItems?.RegisterTabItem(this);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            CheckActivated();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                rendered = true;
                CheckActivated();
                StateHasChanged();
            }
        }

        public void Dispose()
        {
            TabItems?.UnregisterTabItem(this);
            GC.SuppressFinalize(this);
        }

        private void CheckActivated()
        {
            if (!rendered)
            {
                return;
            }

            if (activated)
            {
                return;
            }

            if (TabItems is null || TabItems.ActiveItem != this)
            {
                return;
            }

            activated = true;
        }
    }
}
