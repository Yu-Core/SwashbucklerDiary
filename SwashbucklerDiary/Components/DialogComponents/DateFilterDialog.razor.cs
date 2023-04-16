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
        [EditorRequired]
        [Parameter]
        public Func<TValue, DateOnly> DateOnlyFunc { get; set; } = default!;
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
                return MinDate == DateOnly.MinValue ? I18n.T("Log.Start time") : ((DateOnly)MinDate).ToString("yyyy-MM-dd");
            }
        }
        private string MaxDateText
        {
            get
            {
                return MaxDate == DateOnly.MinValue ? I18n.T("Log.End time") : ((DateOnly)MaxDate).ToString("yyyy-MM-dd");
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
            await OnReset.InvokeAsync();
        }

        private async Task HandleOnOK()
        {
            await InternalValueChanged(false);
            await SetExpression();
            FormBackups = Form.DeepCopy();
            await OnOK.InvokeAsync();
        }

        private async Task SetExpression()
        {
            Expression<Func<TValue, bool>>? exp = null;
            string? defaultDate = DefaultDate?.ToString();

            if (!string.IsNullOrEmpty(defaultDate))
            {
                DateOnly date = DefaultDates[defaultDate];
                exp = exp.And(it => DateOnlyFunc.Invoke(it) >= date);
            }
            else
            {
                if(MinDate != DateOnly.MinValue)
                {
                    exp = exp.And( it => DateOnlyFunc.Invoke(it) >= MinDate);
                }
                
                if(MaxDate != DateOnly.MinValue)
                {
                    exp = exp.And(it => DateOnlyFunc.Invoke(it) <= MaxDate);
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
