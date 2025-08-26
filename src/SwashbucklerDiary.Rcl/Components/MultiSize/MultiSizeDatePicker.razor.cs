using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MultiSizeDatePicker : DialogComponentBase
    {
        private DateOnly mobileMin;

        private DateOnly mobileMax;

        private DateOnly? desktopMin;

        private DateOnly? desktopMax;

        [Parameter]
        public DateOnly Value { get; set; }

        [Parameter]
        public EventCallback<DateOnly> ValueChanged { get; set; }

        [Parameter]
        public DateOnly? Min { get; set; }

        [Parameter]
        public DateOnly? Max { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            mobileMin = (Min is null || Min == default) ? DateOnly.FromDateTime(DateTime.Now.AddYears(-100)) : Min.Value;
            desktopMin = Min == default(DateOnly) ? null : Min;
            mobileMax = (Max is null || Max == default) ? DateOnly.FromDateTime(DateTime.Now.AddYears(100)) : Max.Value;
            desktopMax = Max == default(DateOnly) ? null : Max;
        }

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
