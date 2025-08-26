using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MultiSizeTimePicker : DialogComponentBase
    {
        [Parameter]
        public TimeOnly Value { get; set; }

        [Parameter]
        public EventCallback<TimeOnly> ValueChanged { get; set; }

        private async Task UpdateDisplay(bool value)
        {
            if (Visible)
            {
                await InternalVisibleChanged(false);
            }
        }

        private async Task InternalValueChanged(TimeOnly value)
        {
            Value = value;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(value);
            }
        }
    }
}