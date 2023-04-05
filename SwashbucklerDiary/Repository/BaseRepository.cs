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
            // 创建表
            Type[] types = {
                typeof(DiaryModel),
                typeof(TagModel),
                typeof(DiaryTagModel),
                typeof(UserAchievementModel),
                typeof(UserStateModel),
                typeof(LocationModel),
                typeof(LogModel),
            };
            base.Context!.CodeFirst.InitTables(types);
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

        public virtual Task<List<T>> GetListTakeAsync(int count, Expression<Func<T, bool>> func)
        {
            return base.Context.Queryable<T>()
                .Where(func)
                .Take(count)
                .ToListAsync();
        }
    }
}
