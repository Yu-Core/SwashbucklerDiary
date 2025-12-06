using System.Linq.Expressions;

namespace SwashbucklerDiary.Shared
{
    public class ExpressionBuilder<T> where T : class, new()
    {
        private Expression<Func<T, bool>> _exp = null;

        public ExpressionBuilder<T> And(Expression<Func<T, bool>> exp)
        {
            if (_exp == null)
            {
                _exp = exp;
            }
            else
            {
                ParameterExpression p = Expression.Parameter(typeof(T), _exp.Parameters.First().Name);
                var visitor = new MyExpressionVisitor(p);
                Expression bodyone = visitor.Visit(_exp.Body);
                Expression bodytwo = visitor.Visit(exp.Body);
                _exp = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(bodyone, bodytwo), p);
            }

            return this;
        }

        public ExpressionBuilder<T> AndIF(bool isAnd, Expression<Func<T, bool>> exp)
        {
            if (isAnd)
            {
                And(exp);
            }

            return this;
        }

        public ExpressionBuilder<T> Or(Expression<Func<T, bool>> exp)
        {
            if (_exp == null)
            {
                _exp = exp;
            }
            else
            {
                ParameterExpression p = Expression.Parameter(typeof(T), _exp.Parameters.First().Name);
                var visitor = new MyExpressionVisitor(p);
                Expression bodyone = visitor.Visit(_exp.Body);
                Expression bodytwo = visitor.Visit(exp.Body);
                _exp = Expression.Lambda<Func<T, bool>>(Expression.OrElse(bodyone, bodytwo), p);
            }

            return this;
        }

        public ExpressionBuilder<T> OrIF(bool isOr, Expression<Func<T, bool>> exp)
        {
            if (isOr)
            {
                Or(exp);
            }

            return this;
        }

        public Expression<Func<T, bool>> ToExpression(bool ifNullReturnValue = true)
        {
            if (_exp == null)
            {
                if (ifNullReturnValue)
                {
                    _exp = (T it) => true;
                }
                else
                {
                    _exp = (T it) => false;
                }
            }

            return _exp;
        }

        private class MyExpressionVisitor : ExpressionVisitor
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

    public class ExpressionBuilder
    {
        public static ExpressionBuilder<T> Create<T>() where T : class, new()
        {
            return new ExpressionBuilder<T>();
        }
    }
}
