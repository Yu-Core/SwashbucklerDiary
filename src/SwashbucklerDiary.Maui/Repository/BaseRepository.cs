using SqlSugar;
using SwashbucklerDiary.Maui.IRepository;

namespace SwashbucklerDiary.Maui.Repository
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
    }
}
