using Microsoft.AspNetCore.Components;
using OneOf;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class DatePickerDialog : ShowContentDialogComponentBase
    {
        private DateOnly _date = DateOnly.FromDateTime(DateTime.Now);

        private DateOnly internalDate;

        [Parameter]
        public override bool Visible { get; set; }

        [Parameter]
        public DateOnly Value
        {
            get => _date == default ? DateOnly.FromDateTime(DateTime.Now) : _date;
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
        public OneOf<string, Func<DateOnly, string>, Func<DateOnly, string[]>>? EventColor { get; set; }

        protected override async Task BeforeShowContent()
        {
            await base.BeforeShowContent();

            internalDate = Value;
        }

        private async Task HandleOnOK()
        {
            Value = internalDate;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }

            await InternalVisibleChanged(false);
        }
    }
}
