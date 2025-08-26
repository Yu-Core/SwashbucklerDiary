using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MultiSizeMenu : DialogComponentBase
    {
        private CustomMBottomSheet? mBottomSheetExtension;

        private MMenu? mMenu;

        private List<DynamicListItem> previousDynamicListItems = [];

        protected Dictionary<string, object> previousActivatorAttributes = [];

        [Inject]
        private MasaBlazor MasaBlazor { get; set; } = default!;

        [Parameter]
        public RenderFragment<ActivatorProps> ActivatorContent { get; set; } = default!;

        [Parameter]
        public List<DynamicListItem> DynamicListItems { get; set; } = [];

        [Parameter]
        public Dictionary<string, object> ActivatorAttributes { get; set; } = [];

        [Parameter]
        public bool MenuOffsetX { get; set; }

        [Parameter]
        public StringNumber? MenuNudgeLeft { get; set; }

        [Parameter]
        public StringNumber? MenuNudgeBottom { get; set; }

        public Dictionary<string, object> InternalActivatorAttributes
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

            if (previousActivatorAttributes != ActivatorAttributes)
            {
                //清除旧的Activator的属性，直接Clear是无效的
                foreach (var key in previousActivatorAttributes.Keys)
                {
                    previousActivatorAttributes[key] = false;
                }

                foreach (var item in InternalActivatorAttributes)
                {
                    ActivatorAttributes[item.Key] = item.Value;
                }

                previousActivatorAttributes = ActivatorAttributes;
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

                var props = new ActivatorProps(InternalActivatorAttributes);
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
