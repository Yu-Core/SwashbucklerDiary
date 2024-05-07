using SqlSugar;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Repository
{
    public class LocationRepository : BaseRepository<LocationModel>, ILocationRepository
    {
        public LocationRepository(ISqlSugarClient context) : base(context)
        {
        }

        public override Task<List<LocationModel>> GetListAsync()
        {
            return base.Context.Queryable<LocationModel>()
                .OrderByDescending(it => it.CreateTime)
                .ToListAsync();
        }

        public override Task<List<LocationModel>> GetListAsync(Expression<Func<LocationModel, bool>> expression)
        {
            return base.Context.Queryable<LocationModel>()
                .Where(expression)
                .OrderByDescending(it => it.CreateTime)
                .ToListAsync();
        }
    }
}
