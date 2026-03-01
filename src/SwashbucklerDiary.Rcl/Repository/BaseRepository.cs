using SqlSugar;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Repository
{
    public class BaseRepository<T> : SimpleClient<T>, IBaseRepository<T> where T : class, new()
    {
        public BaseRepository(ISqlSugarClient context) : base(context)
        {
        }

        public override ISqlSugarClient Context
        {
            get
            {
                if (base.Context is SqlSugarScope)
                {
                    return base.Context;
                }
                else
                {
                    return base.Context.CopyNew();
                }
            }
            set => base.Context = value;
        }

        public virtual Task<int> CountAsync()
        {
            return Context.Queryable<T>().CountAsync();
        }

        public Task<bool> DeleteAsync()
        {
            return Context.Deleteable<T>().ExecuteCommandHasChangeAsync();
        }

        public Task<bool> UpdateAsync(T entity, Expression<Func<T, object>> columns)
        {
            return Context.Updateable(entity).UpdateColumns(columns).ExecuteCommandHasChangeAsync();
        }

        public Task<bool> UpdateAsync(List<T> updateObjs, Expression<Func<T, object>> columns)
        {
            return Context.Updateable(updateObjs).UpdateColumns(columns).ExecuteCommandHasChangeAsync();
        }
    }
}
