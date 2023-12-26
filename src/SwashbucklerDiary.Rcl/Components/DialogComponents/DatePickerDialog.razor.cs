using Microsoft.AspNetCore.Components;
using OneOf;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class DatePickerDialog : DialogComponentBase
    {
        private bool _visible;

        private DateOnly _date = DateOnly.FromDateTime(DateTime.Now);

        private DateOnly _internalDate;

        private DateOnly InternalDate
        {
            get => _internalDate;
            set
            {
                _internalDate = (value == DateOnly.MinValue || value == DateOnly.MaxValue) ? DateOnly.FromDateTime(DateTime.Now) : value;
            }
        }

        [Parameter]
        public override bool Visible
        {
            get => _visible;
            set => SetVisible(value);
        }

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

        [Parameter]
        public OneOf<DateOnly[], Func<DateOnly, bool>>? Events { get; set; }

        [Parameter]
        public OneOf<string, Func<DateOnly, string>, Func<DateOnly, string[]>>? EventColor {get;set;}

        private void SetVisible(bool value)
        {
            if (value != Visible)
            {
                if (value)
                {
                    InternalDate = Value;
                }
                _visible = value;
            }
        }

        private async Task HandleOnOK()
        {
            Value = InternalDate;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
            await InternalVisibleChanged(false);
        }
    }
}
