using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Services
{
    public class ResourceService : BaseService<ResourceModel>, IResourceService
    {
        private readonly IResourceRepository _resourceRepository;
        public ResourceService(IResourceRepository resourceRepository)
        {
            base._iBaseRepository = resourceRepository;
            _resourceRepository = resourceRepository;
        }

        public Task<bool> DeleteUnusedResourcesAsync(Expression<Func<ResourceModel, bool>> func)
        {
            return _resourceRepository.DeleteUnusedResourcesAsync(func);
        }
    }
}
