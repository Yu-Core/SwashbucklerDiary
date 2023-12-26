using SqlSugar;
using SwashbucklerDiary.Maui.IRepository;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Repository
{
    public class LocationRepository : BaseRepository<LocationModel>, ILocationRepository
    {
        public LocationRepository(ISqlSugarClient context) : base(context)
        {
        }
    }
}
