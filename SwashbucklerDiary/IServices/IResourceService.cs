using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.IServices
{
    public interface IResourceService : IBaseService<ResourceModel>
    {
        Task<bool> DeleteUnusedResourcesAsync(Expression<Func<ResourceModel, bool>> func);

        List<ResourceModel> GetDiaryResources(string content);

        ResourceType GetResourceType(string uri);
    }
}
