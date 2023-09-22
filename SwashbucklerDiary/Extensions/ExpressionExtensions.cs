using System.Linq.Expressions;

namespace SwashbucklerDiary.Extensions
{
    public static class ExpressionExtensions
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
                ParameterExpression p = Expression.Parameter(typeof(T), left.Parameters.First().Name);
                var visitor = new MyExpressionVisitor(p);
                Expression bodyone = visitor.Visit(left.Body);
                Expression bodytwo = visitor.Visit(right.Body);
                exp = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(bodyone, bodytwo), p);
            }

            return exp.ToExpression();
        }

        public static Expression<Func<T, bool>> AndIF<T>(this Expression<Func<T, bool>> left, bool isAnd, Expression<Func<T, bool>> right)
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
                ParameterExpression p = Expression.Parameter(typeof(T), left.Parameters.First().Name);
                var visitor = new MyExpressionVisitor(p);
                Expression bodyone = visitor.Visit(left.Body);
                Expression bodytwo = visitor.Visit(right.Body);
                exp = Expression.Lambda<Func<T, bool>>(Expression.OrElse(bodyone, bodytwo), p);
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

    public class MyExpressionVisitor : ExpressionVisitor
    {
        public ParameterExpression Parameter { get; set; }

        public MyExpressionVisitor(ParameterExpression parameter)
        {
            Parameter = parameter;
        }
        protected override Expression VisitParameter(ParameterExpression p)
        {
            return Parameter;
        }
#nullable disable
        public override Expression Visit(Expression node)
        {
            return base.Visit(node);//Visit会根据VisitParameter()方法返回的Expression修改这里的node变量
        }
    }
}
