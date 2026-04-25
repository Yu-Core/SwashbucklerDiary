using SqlSugar;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Repository
{
    public class LocationRepository : BaseRepository<LocationModel>, ILocationRepository
    {
        public LocationRepository(ISqlSugarClient context,
            ISettingService settingService) : base(context, settingService)
        {
        }

        public override Task<List<LocationModel>> GetListAsync()
        {
            return Context.Queryable<LocationModel>()
                .OrderByDescending(it => it.CreateTime)
                .ToListAsync();
        }

        public override Task<List<LocationModel>> GetListAsync(Expression<Func<LocationModel, bool>> expression)
        {
            return Context.Queryable<LocationModel>()
                .Where(expression)
                .OrderByDescending(it => it.CreateTime)
                .ToListAsync();
        }
    }
}
