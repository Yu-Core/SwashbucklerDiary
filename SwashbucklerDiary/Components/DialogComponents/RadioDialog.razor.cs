using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Components
{
    public partial class RadioDialog<TItemValue> : DialogComponentBase
    {
        private bool AfterRender;

        [Parameter]
        public TItemValue ItemValue { get; set; } = default!;

        [Parameter]
        public EventCallback<TItemValue> ItemValueChanged { get; set; }

        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public Dictionary<string, TItemValue> Items { get; set; } = new();

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                AfterRender = true;
            }

            base.OnAfterRender(firstRender);
        }

        private string MRadioColor => ThemeService.Dark ? "white" : "black";

        private async Task InternalItemValueChanged(TItemValue value)
        {
            if (!AfterRender || EqualityComparer<TItemValue>.Default.Equals(ItemValue, value))
            {
                return;
            }

            ItemValue = value;
            if (ItemValueChanged.HasDelegate)
            {
                await ItemValueChanged.InvokeAsync(value);
            }
        }
    }
}
