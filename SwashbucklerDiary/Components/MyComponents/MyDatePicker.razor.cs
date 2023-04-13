using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Components
{
    public partial class MyDatePicker : DialogComponentBase
    {
        private bool _value;
        private DateOnly _internalDate;
        private DateOnly InternalDate
        {
            get => _internalDate;
            set
            {
                _internalDate = value == DateOnly.MinValue ? DateOnly.FromDateTime(DateTime.Now) : value;
            }
        }

        [Parameter]
        public override bool Value
        {
            get => _value;
            set => SetValue(value);
        }
        [Parameter]
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        [Parameter]
        public EventCallback<DateOnly> DateChanged { get; set; }

        private void SetValue(bool value)
        {
            if (value != Value)
            {
                if (value)
                {
                    InternalDate = Date;
                }
                _value = value;
            }
        }

        private async Task HandleOnOK()
        {
            Date = InternalDate;
            if (DateChanged.HasDelegate)
            {
                await DateChanged.InvokeAsync(Date);
            }
            await InternalValueChanged(false);
        }
    }
}
