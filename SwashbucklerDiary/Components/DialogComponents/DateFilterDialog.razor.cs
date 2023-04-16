using BlazorComponent;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Extend;
using SwashbucklerDiary.Models.Form;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Components
{
    public partial class DateFilterDialog<TValue> : FilterDialogComponentBase<TValue>
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
        [EditorRequired]
        [Parameter]
        public Func<TValue, DateOnly> Func { get; set; } = default!;
        [Parameter]
        public EventCallback OnUpdate { get; set; }

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
                return MaxDate == DateOnly.MinValue ? I18n.T("Filter.End time") : ((DateOnly)MaxDate).ToString("yyyy-MM-dd");
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
            MaxDate = DateOnly.MinValue;
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
            Expression = null;
            if (ExpressionChanged.HasDelegate)
            {
                await ExpressionChanged.InvokeAsync(Expression);
            }
            await OnUpdate.InvokeAsync();
        }

        private async Task HandleOnOK()
        {
            await InternalValueChanged(false);
            await SetExpression();
            FormBackups = Form.DeepCopy();
            await OnUpdate.InvokeAsync();
        }

        private async Task SetExpression()
        {
            Expression<Func<TValue, bool>>? exp = null;
            string? defaultDate = DefaultDate?.ToString();

            if (!string.IsNullOrEmpty(defaultDate))
            {
                DateOnly date = DefaultDates[defaultDate];
                exp = exp.And(it => Func.Invoke(it) >= date);
            }
            else
            {
                if(MinDate != DateOnly.MinValue)
                {
                    exp = exp.And( it => Func.Invoke(it) >= MinDate);
                }
                
                if(MaxDate != DateOnly.MinValue)
                {
                    exp = exp.And(it => Func.Invoke(it) <= MaxDate);
                }
            }

            Expression = exp;
            if (ExpressionChanged.HasDelegate)
            {
                await ExpressionChanged.InvokeAsync(Expression);
            }
        }
    }
}
