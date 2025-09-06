using SqlSugar;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class ExpressionableExtensions
    {
        public static Expression<Func<T, bool>> ToExpression<T>(this Expressionable<T> @this, bool ifNullReturnValue) where T : class, new()
        {
            var exp = @this.ToExpression();

            if (ifNullReturnValue == false && exp.Body.ToString() == "True")
            {
                exp = it => false;
            }

            return exp;
        }
    }
}
