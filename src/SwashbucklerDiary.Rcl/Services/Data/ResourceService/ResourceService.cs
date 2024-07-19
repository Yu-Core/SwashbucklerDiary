using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Services
{
    public class ResourceService : BaseDataService<ResourceModel>, IResourceService
    {
        protected readonly IResourceRepository _resourceRepository;

        protected readonly IMediaResourceManager _mediaResourceManager;

        public ResourceService(IResourceRepository resourceRepository,
            IMediaResourceManager mediaResourceManager)
        {
            base._iBaseRepository = resourceRepository;
            _resourceRepository = resourceRepository;
            _mediaResourceManager = mediaResourceManager;
        }

        public async Task<bool> DeleteUnusedResourcesAsync(Expression<Func<ResourceModel, bool>> expression)
        {
            var resources = await _resourceRepository.QueryUnusedResourcesAsync(expression);
            if (resources is null || resources.Count == 0)
            {
                return false;
            }

            var flag = await _resourceRepository.DeleteAsync(resources);
            if (!flag)
            {
                return false;
            }

            DeleteResourceFiles(resources);
            return true;
        }

        private void DeleteResourceFiles(List<ResourceModel> resources)
        {
            foreach (var resource in resources)
            {
                var path = _mediaResourceManager.UrlRelativePathToFilePath(resource.ResourceUri!);
                if (!string.IsNullOrEmpty(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
