using SwashbucklerDiary.Maui.BlazorWebView;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Repository;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Maui.Services
{
    public class ResourceService : BaseDataService<ResourceModel>, IResourceService
    {
        private readonly IResourceRepository _resourceRepository;

        
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

            foreach (var resource in resources)
            {
                var path = MauiBlazorWebViewHandler.UrlRelativePathToFilePath(resource.ResourceUri!);
                if (!string.IsNullOrEmpty(path))
                {
                    File.Delete(path);
                }
            }
            return true;
        }
    }
}
