using BlazorComponent.I18n;
using SqlSugar;
using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Repository
{
    public class BaseRepository<T> : SimpleClient<T>, IBaseRepository<T> where T : class, new()
    {
        public BaseRepository(ISqlSugarClient context) : base(context)
        {
            base.Context = context;
            // 创建表
            Type[] types = { typeof(DiaryModel),typeof(TagModel),typeof(DiaryTagModel) };
            base.Context!.CodeFirst.InitTables(types);
        }

        public virtual Task<int> CountAsync()
        {
            return base.Context.Queryable<T>().CountAsync();
        }

        public virtual Task<List<T>> GetListTakeAsync(int count)
        {
            return base.Context.Queryable<T>().Take(count).ToListAsync();
        }
    }
}
