using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MultiMenu : DialogComponentBase
    {
        private MBottomSheetExtension? mBottomSheetExtension;

        private MMenu? mMenu;

        private List<DynamicListItem> previousDynamicListItems = [];

        [Inject]
        private MasaBlazor MasaBlazor { get; set; } = default!;

        [Parameter]
        public RenderFragment<ActivatorProps> ActivatorContent { get; set; } = default!;

        [Parameter]
        public List<DynamicListItem> DynamicListItems { get; set; } = [];

        public Dictionary<string, object> ActivatorAttributes
            => (IsDesktop ? mMenu?.ActivatorAttributes : mBottomSheetExtension?.ActivatorAttributes) ?? [];

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (previousDynamicListItems != DynamicListItems)
            {
                DynamicListItems.ForEach(item =>
                {
                    var onClick = item.OnClick;
                    item.OnClick = EventCallback.Factory.Create(this, () => HandleOnContentClick(onClick));
                });
                previousDynamicListItems = DynamicListItems;
            }
        }

        protected RenderFragment? ComputedActivatorContent
        {
            get
            {
                if (ActivatorContent is null)
                {
                    return null;
                }

                var props = new ActivatorProps(ActivatorAttributes);
                return ActivatorContent(props);
            }
        }

        private bool IsDesktop => MasaBlazor.Breakpoint.SmAndUp;

        private async Task UpdateDisplay(bool value)
        {
            if (!Visible)
            {
                return;
            }

            if (IsDesktop)
            {
                if (mBottomSheetExtension is not null)
                {
                    await mBottomSheetExtension.HandleOnOutsideClickAsync();
                }
            }
            else
            {
                if (mMenu is not null)
                {
                    await mMenu.HandleOnOutsideClickAsync();
                }
            }
        }

        private async Task HandleOnContentClick(EventCallback eventCallback)
        {
            await InternalVisibleChanged(false);
            StateHasChanged();
            await eventCallback.InvokeAsync();
        }
    }
}
