using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class DateFilterDialog : DialogComponentBase
    {
        private bool _visible;

        private bool showMinDate;

        private bool showMaxDate;

        private DateFilterForm internalForm = new();

        [Parameter]
        public override bool Visible
        {
            get => _visible;
            set => SetVisible(value);
        }

        [Parameter]
        public DateFilterForm Value { get; set; } = new();

        [Parameter]
        public EventCallback<DateFilterForm> ValueChanged { get; set; }

        [Parameter]
        public EventCallback OnOK { get; set; }

        [Parameter]
        public EventCallback OnReset { get; set; }

        private static Dictionary<string, DateOnly> DefaultDates
            => DateFilterForm.DefaultDates;

        private StringNumber DefaultDate
        {
            get => internalForm.DefaultDate;
            set
            {
                internalForm.DefaultDate = value?.ToString() ?? string.Empty;
            }
        }

        private DateOnly MinDate
        {
            get => internalForm.MinDate;
            set => internalForm.MinDate = value;
        }

        private DateOnly MaxDate
        {
            get => internalForm.MaxDate;
            set => internalForm.MaxDate = value;
        }

        private string MinDateText
            => MinDate == default ? I18n.T("Filter.Start time") : MinDate.ToString("d").Replace('/', '-');

        private string MaxDateText
            => MaxDate == default ? I18n.T("Filter.End time") : MaxDate.ToString("d").Replace('/', '-');

        private static DateOnly Today => DateOnly.FromDateTime(DateTime.Now);

        private void SetVisible(bool value)
        {
            if (value != Visible)
            {
                _visible = value;
                if (value)
                {
                    internalForm = Value.DeepCopy();
                }
            }
        }

        private void SelectDeafultDate()
        {
            MinDate = default;
            MaxDate = default;
        }

        private void OpenMinDateDialog()
        {
            showMinDate = true;
            DefaultDate = string.Empty;
        }

        private void OpenMaxDateDialog()
        {
            showMaxDate = true;
            DefaultDate = string.Empty;
        }

        private async Task HandleOnReset()
        {
            internalForm = new();
            Value = new();
            await ValueChanged.InvokeAsync(Value);
            await OnReset.InvokeAsync();
        }

        private async Task HandleOnOK()
        {
            await InternalVisibleChanged(false);
            Value = internalForm.DeepCopy();
            await ValueChanged.InvokeAsync(Value);
            await OnOK.InvokeAsync();
        }

    }
}
