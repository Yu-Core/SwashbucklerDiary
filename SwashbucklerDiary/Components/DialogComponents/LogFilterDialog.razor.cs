using BlazorComponent;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class LogFilterDialog : DialogComponentBase
    {
        private bool _value;
        public bool _isFilter;
        private bool ShowMinDate;
        private bool ShowMaxDate;
        private Model model = new();
        private readonly Dictionary<string, DateOnly> DefaultDates = new()
        {
            {"Log.Last month",DateOnly.FromDateTime(DateTime.Now.AddMonths(-1))},
            {"Log.Last three months",DateOnly.FromDateTime(DateTime.Now.AddMonths(-3))},
            {"Log.Last six months",DateOnly.FromDateTime(DateTime.Now.AddMonths(-6))},
        };

        [Parameter]
        public override bool Value
        {
            get => _value;
            set => SetValue(value);
        }
        [Parameter]
        public List<LogModel> Logs { get; set; } = new();
        [Parameter]
        public bool IsFilter
        {
            get => _isFilter;
            set => SetIsFilter(value);
        }
        [Parameter]
        public EventCallback<bool> IsFilterChanged { get; set; }
        [Parameter]
        public EventCallback OnReset { get; set; }
        [Parameter]
        public EventCallback<List<LogModel>> OnOK { get; set; }

        private class Model
        {
            public StringNumber DefaultDate { get; set; } = string.Empty;
            public DateOnly MinDate { get; set; }
            public DateOnly MaxDate { get; set; }
        }
        private StringNumber DefaultDate { get; set; } = string.Empty;
        private DateOnly MinDate { get; set; }
        private DateOnly MaxDate { get; set; }
        private Func<LogModel, bool> DateRange
        {
            get
            {
                string? defaultDate = DefaultDate?.ToString();
                if (!string.IsNullOrEmpty(defaultDate))
                {
                    return it => DateOnly.FromDateTime(it.Timestamp) >= DefaultDates[defaultDate];
                }

                if (MinDate != DateOnly.MinValue && MaxDate != DateOnly.MinValue)
                {
                    return it => DateOnly.FromDateTime(it.Timestamp) >= MinDate && DateOnly.FromDateTime(it.Timestamp) <= MaxDate;
                }

                if (MinDate != DateOnly.MinValue)
                {
                    return it => DateOnly.FromDateTime(it.Timestamp) >= MinDate;
                }

                if (MaxDate != DateOnly.MaxValue)
                {
                    return it => DateOnly.FromDateTime(it.Timestamp) <= MaxDate;
                }

                return it => true;
            }
        }
        private string MinDateText
        {
            get
            {
                return MinDate == DateOnly.MinValue ? I18n.T("Log.Start time") : MinDate.ToString("yyyy-MM-dd");
            }
        }
        private string MaxDateText
        {
            get
            {
                return MaxDate == DateOnly.MinValue ? I18n.T("Log.End time") : MaxDate.ToString("yyyy-MM-dd");
            }
        }

        private void SetValue(bool value)
        {
            if (value != Value)
            {
                if (value)
                {
                    MinDate = model.MinDate;
                    MaxDate = model.MaxDate;
                    DefaultDate = model.DefaultDate;
                }
                _value = value;
            }
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
            
            await FilterChanged(false);
            await OnReset.InvokeAsync();
        }
        private async Task HandleOnOK()
        {
            if(!string.IsNullOrEmpty(DefaultDate?.ToString()) || MaxDate != DateOnly.MinValue || MinDate != DateOnly.MinValue)
            {
                await FilterChanged(true);
                var logs = Logs.Where(DateRange).ToList();
                await OnOK.InvokeAsync(logs);
            }
            else
            {
                await FilterChanged(false);
                await OnOK.InvokeAsync(Logs);
            }
            await InternalValueChanged(false);
        }

        private async Task FilterChanged(bool value)
        {
            IsFilter = value;
            if (IsFilterChanged.HasDelegate)
            {
                await IsFilterChanged.InvokeAsync(value);
            }
        }

        private void SelectDeafultDate()
        {
            MaxDate = DateOnly.MinValue;
            MinDate = DateOnly.MinValue;
        }

        private void SetIsFilter(bool value)
        {
            if (value)
            {
                model.DefaultDate = DefaultDate ?? string.Empty;
                model.MaxDate = MaxDate;
                model.MinDate = MinDate;
            }
            else
            {
                model.DefaultDate = DefaultDate = string.Empty;
                model.MaxDate = MaxDate = DateOnly.MinValue;
                model.MinDate = MinDate = DateOnly.MinValue;
            }
            _isFilter = value;
        }
    }
}
