using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Extend;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Components
{
    public partial class SearchFilter<TValue> : FilterDialogComponentBase<TValue>
    {
        private bool _value;
        public string? Search;

        [Parameter]
        public override bool Value
        {
            get => _value;
            set => SetValue(value);
        }
        [Parameter]
        public string? Title { get; set; }
        [EditorRequired]
        [Parameter]
        public Func<TValue, string,bool> Func { get; set; } = default!;
        [Parameter]
        public EventCallback OnUpdate { get; set; }

        private void SetValue(bool value)
        {
            if (_value != value)
            {
                _value = value;
                Task.Run(() =>
                {
                    if (value)
                    {
                        NavigateService.Action += CloseSearch;
                    }
                    else
                    {
                        Search = string.Empty;
                        NavigateService.Action -= CloseSearch;
                    }
                });
            }
        }

        private async void CloseSearch()
        {
            await InternalValueChanged(false);
        }

        private async Task SearchChanged(string? value)
        {
            Search = value;

            await SetExpression();
            await OnUpdate.InvokeAsync();
        }

        private async Task SetExpression()
        {
            Expression<Func<TValue, bool>>? exp = null;
            if (!string.IsNullOrWhiteSpace(Search))
            {
                exp = exp.And(it => Func.Invoke(it,Search));
            }

            Expression = exp;
            if (ExpressionChanged.HasDelegate)
            {
                await ExpressionChanged.InvokeAsync(Expression);
            }
        }
    }
}
