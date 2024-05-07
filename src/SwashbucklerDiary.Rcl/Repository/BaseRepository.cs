using SqlSugar;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Repository
{
    public class BaseRepository<T> : SimpleClient<T>, IBaseRepository<T> where T : class, new()
    {
        public BaseRepository(ISqlSugarClient context) : base(context)
        {
            base.Context = context;
        }

        public virtual Task<int> CountAsync()
        {
            return base.Context.Queryable<T>().CountAsync();
        }

        public Task<bool> DeleteAsync()
        {
            return base.Context.Deleteable<T>().ExecuteCommandHasChangeAsync();
        }

        public Task<bool> UpdateAsync(T entity, Expression<Func<T, object>> columns)
        {
            return base.Context.Updateable(entity).UpdateColumns(columns).ExecuteCommandHasChangeAsync();
        }
    }
}
