using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Components
{
    public class FilterDialogComponentBase<TValue> : DialogComponentBase 
    {
        [Parameter]
        public Expression<Func<TValue, bool>>? Expression { get; set; }
        [Parameter]
        public EventCallback<Expression<Func<TValue, bool>>> ExpressionChanged { get; set; }
    }
}
