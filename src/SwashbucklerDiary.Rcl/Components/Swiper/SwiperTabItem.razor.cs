using Masa.Blazor;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SwiperTabItem : IDisposable
    {
        private bool isActivated;

        private bool isRendered;

        [CascadingParameter]
        public SwiperTabItems? TabItems { get; set; }

        [Parameter]
        public string Id { get; set; } = $"B-{Guid.NewGuid()}";

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        public StringNumber? Value { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            TabItems?.RegisterTabItem(this);
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            await ActivateAsync(true);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                isRendered = true;
                await ActivateAsync();
                StateHasChanged();
            }
        }

        public void Dispose()
        {
            TabItems?.UnregisterTabItem(this);
            GC.SuppressFinalize(this);
        }

        private async Task ActivateAsync(bool needWait = false)
        {
            if (!isRendered)
            {
                return;
            }

            if (isActivated)
            {
                return;
            }

            if (TabItems is null || TabItems.ActiveItem != this)
            {
                return;
            }

            if (needWait)
            {
                await Task.Delay(300);
            }

            isActivated = true;
        }
    }
}
