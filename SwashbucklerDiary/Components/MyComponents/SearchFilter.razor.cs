using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Extend;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Components
{
    public partial class SearchFilter<TValue> : FilterDialogComponentBase<TValue>
    {
        private bool _value;

        [Parameter]
        public override bool Value
        {
            get => _value;
            set => SetValue(value);
        }
        [Parameter]
        public string? Title { get; set; }
        [Parameter]
        public string? Search { get; set; }
        [Parameter]
        public EventCallback<string> SearchChanged { get;set; }
        [EditorRequired]
        [Parameter]
        public Func<TValue, string> Func { get; set; } = default!;
        [Parameter]
        public EventCallback OnUpdate { get; set; }

        private void SetValue(bool value)
        {
            if (_value != value)
            {
                _value = value;
                Task.Run(async () =>
                {
                    if (value)
                    {
                        NavigateService.Action += CloseSearch;
                    }
                    else
                    {
                        Search = string.Empty;
                        await InternalSearchChanged(Search);
                        NavigateService.Action -= CloseSearch;
                    }
                });
            }
        }

        private async void CloseSearch()
        {
            await InternalValueChanged(false);
        }

        private async Task InternalSearchChanged(string? value)
        {
            Search = value;
            if (SearchChanged.HasDelegate)
            {
                await SearchChanged.InvokeAsync(Search);
            }
            await SetExpression();
            await OnUpdate.InvokeAsync();
        }

        private async Task SetExpression()
        {
            Expression<Func<TValue, bool>>? exp = null;
            if (!string.IsNullOrWhiteSpace(Search))
            {
                exp = exp.And(it => Func.Invoke(it).Contains(Search));
            }

            Expression = exp;
            if (ExpressionChanged.HasDelegate)
            {
                await ExpressionChanged.InvokeAsync(Expression);
            }
        }
    }
}
