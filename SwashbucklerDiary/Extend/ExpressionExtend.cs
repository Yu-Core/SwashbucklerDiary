using System.Linq.Expressions;

namespace SwashbucklerDiary.Extend
{
    public static class ExpressionExtend
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>>? left, Expression<Func<T, bool>> right)
        {
            Expression<Func<T, bool>>? exp;
            if (left == null)
            {
                exp = right;
            }
            else
            {
                exp = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left.Body, right.Body), left.Parameters);
            }

            return exp.ToExpression();
        }

        public static Expression<Func<T, bool>> AndIF<T>(this Expression<Func<T, bool>> left,bool isAnd, Expression<Func<T, bool>> right)
        {
            Expression<Func<T, bool>>? exp;
            if (isAnd)
            {
                exp = left.And(right);
            }
            else
            {
                exp = left;
            }

            return exp.ToExpression();
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>>? left, Expression<Func<T, bool>> right)
        {
            Expression<Func<T, bool>>? exp;
            if (left == null)
            {
                exp = right;
            }
            else
            {
                exp = Expression.Lambda<Func<T, bool>>(Expression.OrElse(left.Body, right.Body), left.Parameters);
            }

            return exp.ToExpression();
        }

        public static Expression<Func<T, bool>> OrIF<T>(this Expression<Func<T, bool>> left, bool isOr, Expression<Func<T, bool>> right)
        {
            Expression<Func<T, bool>>? exp;
            if (isOr)
            {
                exp = left.Or(right);
            }
            else
            {
                exp = left;
            }

            return exp.ToExpression();
        }

        public static Expression<Func<T, bool>> ToExpression<T>(this Expression<Func<T, bool>>? exp)
        {
            if (exp == null)
                return it => true;
            return exp;
        }

    }
}
