using SqlSugar;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Repository
{
    public class LocationRepository : BaseRepository<LocationModel>, ILocationRepository
    {
        public LocationRepository(ISqlSugarClient context) : base(context)
        {
        }
    }
}
