using SqlSugar;
using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Repository
{
    public class LocationRepository : BaseRepository<LocationModel>, ILocationRepository
    {
        public LocationRepository(ISqlSugarClient context) : base(context)
        {
        }
    }
}
