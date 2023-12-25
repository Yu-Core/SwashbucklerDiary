using SwashbucklerDiary.Maui.IRepository;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Services
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
