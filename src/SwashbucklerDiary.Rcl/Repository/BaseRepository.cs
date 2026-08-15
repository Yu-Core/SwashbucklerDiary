using SqlSugar;
using SwashbucklerDiary.Rcl.Services;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Repository
{
    public class BaseRepository<T> : SimpleClient<T>, IBaseRepository<T> where T : class, new()
    {
        private readonly ISettingService _settingService;

        public BaseRepository(ISqlSugarClient context, ISettingService settingService) : base(context)
        {
            _settingService = settingService;
        }

        public override ISqlSugarClient Context
        {
            get => Itenant.GetConnection(ConfigId);
            set => base.Context = value;
        }

        private string ConfigId
            => _settingService.GetTemp(it => it.PrivacyMode) ? SQLiteConstants.PrivacyDatabaseFilename : SQLiteConstants.MainDatabaseFilename;

        public ITenant Itenant => base.Context.CopyNew();

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
