using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public class LocationService : BaseDataService<LocationModel>, ILocationService
    {
        private readonly ILocationRepository _iLocationRepository;

        public LocationService(ILocationRepository iLocationRepository)
        {
            base._iBaseRepository = iLocationRepository;
            _iLocationRepository = iLocationRepository;
        }
    }
}
