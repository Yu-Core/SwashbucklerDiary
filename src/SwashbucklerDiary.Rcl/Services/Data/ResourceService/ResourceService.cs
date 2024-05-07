using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Services
{
    public abstract class ResourceService : BaseDataService<ResourceModel>, IResourceService
    {
        protected readonly IResourceRepository _resourceRepository;

        public ResourceService(IResourceRepository resourceRepository)
        {
            base._iBaseRepository = resourceRepository;
            _resourceRepository = resourceRepository;
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

        protected abstract void DeleteResourceFiles(List<ResourceModel> resources);
    }
}
