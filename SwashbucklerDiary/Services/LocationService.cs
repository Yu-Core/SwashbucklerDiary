using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Services
{
    public class LocationService : BaseService<LocationModel>,ILocationService
    {
        private readonly ILocationRepository _iLocationRepository;

        public LocationService(ILocationRepository iLocationRepository)
        {
            base._iBaseRepository = iLocationRepository;
            _iLocationRepository = iLocationRepository;
        }
    }
}
