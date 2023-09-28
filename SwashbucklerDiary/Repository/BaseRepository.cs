using SqlSugar;
using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Repository
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

        public virtual Task<List<T>> GetListTakeAsync(int count)
        {
            return base.Context.Queryable<T>().Take(count).ToListAsync();
        }

        public virtual Task<List<T>> GetListTakeAsync(int count, Expression<Func<T, bool>> expression)
        {
            return base.Context.Queryable<T>()
                .Where(expression)
                .Take(count)
                .ToListAsync();
        }
    }
}
