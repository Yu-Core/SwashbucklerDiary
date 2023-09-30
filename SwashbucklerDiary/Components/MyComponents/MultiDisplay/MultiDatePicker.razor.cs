using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Components
{
    public partial class MultiDatePicker : DialogComponentBase
    {
        private DateOnly _date = DateOnly.FromDateTime(DateTime.Now);

        [Parameter]
        public DateOnly Date
        {
            get => (_date == DateOnly.MinValue || _date == DateOnly.MaxValue) ? DateOnly.FromDateTime(DateTime.Now) : _date;
            set => _date = value;
        }

        [Parameter]
        public EventCallback<DateOnly> DateChanged { get; set; }

        [Parameter]
        public DateOnly? Min { get; set; }

        [Parameter]
        public DateOnly? Max { get; set; }

        private async Task UpdateDisplay(bool value)
        {
            if (Value)
            {
                await InternalValueChanged(false);
            }
        }
    }
}
