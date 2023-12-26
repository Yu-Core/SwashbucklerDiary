using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MultiDatePicker : DialogComponentBase
    {
        private DateOnly _date = DateOnly.FromDateTime(DateTime.Now);

        [Parameter]
        public DateOnly Value
        {
            get => (_date == DateOnly.MinValue || _date == DateOnly.MaxValue) ? DateOnly.FromDateTime(DateTime.Now) : _date;
            set => _date = value;
        }

        [Parameter]
        public EventCallback<DateOnly> ValueChanged { get; set; }

        [Parameter]
        public DateOnly? Min { get; set; }

        [Parameter]
        public DateOnly? Max { get; set; }

        private async Task UpdateDisplay(bool value)
        {
            if (Visible)
            {
                await InternalVisibleChanged(false);
            }
        }
    }
}
