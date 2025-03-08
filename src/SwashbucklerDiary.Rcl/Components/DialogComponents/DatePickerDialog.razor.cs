using Microsoft.AspNetCore.Components;
using OneOf;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class DatePickerDialog : DialogComponentBase
    {
        private DateOnly internalValue;

        [Parameter]
        public DateOnly Value { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [Parameter]
        public EventCallback<DateOnly> ValueChanged { get; set; }

        [Parameter]
        public DateOnly? Min { get; set; }

        [Parameter]
        public DateOnly? Max { get; set; }

        [Parameter]
        public OneOf<DateOnly[], Func<DateOnly, bool>>? Events { get; set; }

        [Parameter]
        public OneOf<string, Func<DateOnly, string>, Func<DateOnly, string[]>>? EventColor { get; set; }

        private void BeforeShowContent()
        {
            internalValue = Value == default ? DateOnly.FromDateTime(DateTime.Now) : Value;
        }

        private async Task HandleOnOK()
        {
            Value = internalValue;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }

            await InternalVisibleChanged(false);
        }
    }
}
