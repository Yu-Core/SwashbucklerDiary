using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class TimePickerDialog : DialogComponentBase
    {
        private TimeOnly? internalValue;

        [Parameter]
        public TimeOnly Value { get; set; }

        [Parameter]
        public EventCallback<TimeOnly> ValueChanged { get; set; }

        private void BeforeShowContent()
        {
            internalValue = Value == default ? TimeOnly.FromDateTime(DateTime.Now) : Value;
        }

        private async Task HandleOnOK()
        {
            Value = internalValue ?? default;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }

            await InternalVisibleChanged(false);
        }
    }
}