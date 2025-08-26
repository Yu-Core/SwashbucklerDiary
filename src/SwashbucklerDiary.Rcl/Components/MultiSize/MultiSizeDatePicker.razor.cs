using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MultiSizeDatePicker : DialogComponentBase
    {
        [Parameter]
        public DateOnly Value { get; set; }

        [Parameter]
        public EventCallback<DateOnly> ValueChanged { get; set; }

        [Parameter]
        public DateOnly? Min { get; set; }

        [Parameter]
        public DateOnly? Max { get; set; }

        private DateOnly MobileMin => Min ?? default;

        private DateOnly MobileMax => Max ?? default;

        private DateOnly? DesktopMin => Min == default(DateOnly) ? null : Min;

        private DateOnly? DesktopMax => Max == default(DateOnly) ? null : Max;

        private async Task UpdateDisplay(bool value)
        {
            if (Visible)
            {
                await InternalVisibleChanged(false);
            }
        }

        private async Task InternalValueChanged(DateOnly value)
        {
            Value = value;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(value);
            }
        }
    }
}
