using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Components
{
    public partial class MyDatePicker : DialogComponentBase
    {
        private bool _value;
        private DateOnly _date = DateOnly.FromDateTime(DateTime.Now);
        private DateOnly _desktopDate;
        private DateOnly DesktopDate
        {
            get => _desktopDate;
            set
            {
                _desktopDate = (value == DateOnly.MinValue || value == DateOnly.MaxValue) ? DateOnly.FromDateTime(DateTime.Now) : value;
            }
        }

        [Parameter]
        public override bool Value
        {
            get => _value;
            set => SetValue(value);
        }
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

        private void SetValue(bool value)
        {
            if (value != Value)
            {
                if (value)
                {
                    DesktopDate = Date;
                }
                _value = value;
            }
        }

        private async Task HandleOnOK()
        {
            Date = DesktopDate;
            if (DateChanged.HasDelegate)
            {
                await DateChanged.InvokeAsync(Date);
            }
            await InternalValueChanged(false);
        }
    }
}
