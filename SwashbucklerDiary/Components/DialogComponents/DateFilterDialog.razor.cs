using BlazorComponent;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Extend;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class DateFilterDialog : DialogComponentBase
    {
        private bool _value;
        private bool ShowMinDate;
        private bool ShowMaxDate;
        private DateFilterForm InternalForm = new();
        private static Dictionary<string, DateOnly> DefaultDates => DateFilterForm.DefaultDates;

        [Parameter]
        public override bool Value
        {
            get => _value;
            set => SetValue(value);
        }
        [Parameter]
        public DateFilterForm Form { get; set; } = new();
        [Parameter]
        public EventCallback<DateFilterForm> FormChanged { get; set; }
        [Parameter]
        public EventCallback OnOK { get; set; }
        [Parameter]
        public EventCallback OnReset { get; set; }

        protected virtual Task DialogAfterRenderAsync()
        {
            InternalForm = Form.DeepCopy();
            return Task.CompletedTask;
        }

        private StringNumber DefaultDate
        {
            get => InternalForm.DefaultDate;
            set
            {
                InternalForm.DefaultDate = value?.ToString() ?? string.Empty;
            }
        }
        private DateOnly MinDate
        {
            get => InternalForm.MinDate;
            set => InternalForm.MinDate = value;
        }
        private DateOnly MaxDate
        {
            get => InternalForm.MaxDate;
            set => InternalForm.MaxDate = value;
        }
        private string MinDateText
            => MinDate == DateOnly.MinValue ? I18n.T("Filter.Start time") : ((DateOnly)MinDate).ToString("yyyy-MM-dd");
        private string MaxDateText
            => MaxDate == DateOnly.MaxValue ? I18n.T("Filter.End time") : ((DateOnly)MaxDate).ToString("yyyy-MM-dd");

        private void SetValue(bool value)
        {
            if (value != Value)
            {
                _value = value;
                if (value)
                {
                    InternalForm = Form.DeepCopy();
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
            InternalForm = new();
            Form = new();
            await FormChanged.InvokeAsync(Form);
            await OnReset.InvokeAsync();
        }

        private async Task HandleOnOK()
        {
            await InternalValueChanged(false);
            Form = InternalForm.DeepCopy();
            await FormChanged.InvokeAsync(Form);
            await OnOK.InvokeAsync();
        }

    }
}
