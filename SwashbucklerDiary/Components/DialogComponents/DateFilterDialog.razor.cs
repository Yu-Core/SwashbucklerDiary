using BlazorComponent;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Extend;
using SwashbucklerDiary.Models.Form;

namespace SwashbucklerDiary.Components
{
    public partial class DateFilterDialog : DialogComponentBase
    {
        private bool _value;
        private bool ShowMinDate;
        private bool ShowMaxDate;
        private DateFilterForm Form = new();
        private DateFilterForm FormBackups = new();
        private readonly Dictionary<string, DateOnly> DefaultDates = new()
        {
            {"Filter.Last month",DateOnly.FromDateTime(DateTime.Now.AddMonths(-1))},
            {"Filter.Last three months",DateOnly.FromDateTime(DateTime.Now.AddMonths(-3))},
            {"Filter.Last six months",DateOnly.FromDateTime(DateTime.Now.AddMonths(-6))},
        };

        [Parameter]
        public override bool Value
        {
            get => _value;
            set => SetValue(value);
        }
        [Parameter]
        public DateOnly DateMinValue { get; set; }
        [Parameter]
        public EventCallback<DateOnly> DateMinValueChanged { get; set; }
        [Parameter]
        public DateOnly DateMaxValue { get; set; }
        [Parameter]
        public EventCallback<DateOnly> DateMaxValueChanged { get; set; }
        [Parameter]
        public EventCallback OnOK { get; set; }
        [Parameter]
        public EventCallback OnReset { get; set; }

        private StringNumber DefaultDate
        {
            get => Form.DefaultDate;
            set
            {
                Form.DefaultDate = value?.ToString() ?? string.Empty;
            }
        }
        private DateOnly MinDate
        {
            get => Form.MinDate;
            set => Form.MinDate = value;
        }
        private DateOnly MaxDate
        {
            get => Form.MaxDate;
            set => Form.MaxDate = value;
        }
        private string MinDateText
        {
            get
            {
                return MinDate == DateOnly.MinValue ? I18n.T("Filter.Start time") : ((DateOnly)MinDate).ToString("yyyy-MM-dd");
            }
        }
        private string MaxDateText
        {
            get
            {
                return MaxDate == DateOnly.MaxValue ? I18n.T("Filter.End time") : ((DateOnly)MaxDate).ToString("yyyy-MM-dd");
            }
        }

        private void SetValue(bool value)
        {
            if (value != Value)
            {
                _value = value;
                if (value)
                {
                    Form = FormBackups.DeepCopy();
                }
            }
        }

        private void SelectDeafultDate()
        {
            MinDate = DateOnly.MinValue;
            MaxDate = DateOnly.MaxValue;
        }

        private void OpenMinDateDialog()
        {
            ShowMinDate = true;
            DefaultDate = string.Empty;
        }

        private void OpenMaxDateDialog()
        {
            ShowMaxDate = true;
            DefaultDate = string.Empty;
        }

        private async Task HandleOnReset()
        {
            FormBackups = new();
            Form = new();
            await Update();
            await OnReset.InvokeAsync();
        }

        private async Task HandleOnOK()
        {
            await InternalValueChanged(false);
            FormBackups = Form.DeepCopy();
            await Update();
            await OnOK.InvokeAsync();
        }

        private async Task Update()
        {
            await UpdateDateMinValue();
            await UpdateDateMaxValue();
        }

        private async Task UpdateDateMinValue()
        {
            DateMinValue = GetDateMinValue();
            if (DateMinValueChanged.HasDelegate)
            {
                await DateMinValueChanged.InvokeAsync(DateMinValue);
            }
        }

        private async Task UpdateDateMaxValue()
        {
            DateMaxValue = GetDateMaxValue();
            if (DateMaxValueChanged.HasDelegate)
            {
                await DateMaxValueChanged.InvokeAsync(DateMaxValue);
            }
        }

        private DateOnly GetDateMinValue()
        {
            string? defaultDate = DefaultDate?.ToString();
            if (!string.IsNullOrEmpty(defaultDate))
            {
                return DefaultDates[defaultDate];
            }

            if (MinDate != DateOnly.MinValue)
            {
                return MinDate;
            }

            return DateOnly.MinValue;
        }
        private DateOnly GetDateMaxValue()
        {
            if(MaxDate != DateOnly.MaxValue)
            {
                return MaxDate;
            }

            return DateOnly.MaxValue;
        }
    }
}
