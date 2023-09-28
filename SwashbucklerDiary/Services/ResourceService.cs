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

        public Task<List<ResourceModel>> DeleteUnusedResourcesAsync(Expression<Func<ResourceModel, bool>> expression)
        {
            return _resourceRepository.DeleteUnusedResourcesAsync(expression);
        }
    }
}
