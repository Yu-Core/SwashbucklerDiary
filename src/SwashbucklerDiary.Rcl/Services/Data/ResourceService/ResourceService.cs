using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Services
{
    public class ResourceService : BaseDataService<ResourceModel>, IResourceService
    {
        protected readonly IResourceRepository _resourceRepository;

        protected readonly IMediaResourceManager _mediaResourceManager;

        protected readonly ISettingService _settingService;

        public ResourceService(IResourceRepository resourceRepository,
            IMediaResourceManager mediaResourceManager,
            ISettingService settingService)
        {
            base._iBaseRepository = resourceRepository;
            _resourceRepository = resourceRepository;
            _mediaResourceManager = mediaResourceManager;
            _settingService = settingService;
        }

        public async Task<bool> DeleteUnusedResourcesAsync(Expression<Func<ResourceModel, bool>> expression)
        {
            bool privacyMode = _settingService.GetTemp(it => it.PrivacyMode);
            var (currentUnusedResourceUris, trulyUnusedResourceUris) = await _resourceRepository.QueryUnusedResourcesAsync(expression, privacyMode);
            if (currentUnusedResourceUris is null || currentUnusedResourceUris.Count == 0)
            {
                return false;
            }

            var flag = await _resourceRepository.DeleteByIdAsync(currentUnusedResourceUris);
            if (!flag)
            {
                return false;
            }

            DeleteResourceFiles(trulyUnusedResourceUris);
            return true;
        }

        public Task<ResourceModel> FindIncludesAsync(string id)
        {
            return _resourceRepository.FindIncludesAsync(id);
        }

        public Task<List<Guid>> GetDiaryIdsAsync(string id)
        {
            return _resourceRepository.GetDiaryIdsAsync(id);
        }

        private void DeleteResourceFiles(List<string?> resourceUris)
        {
            foreach (var resourceUri in resourceUris)
            {
                if (resourceUri is null)
                {
                    continue;
                }

                var path = _mediaResourceManager.UrlRelativePathToFilePath(resourceUri);
                if (!string.IsNullOrEmpty(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
