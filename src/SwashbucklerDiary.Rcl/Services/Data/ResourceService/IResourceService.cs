using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface IResourceService : IBaseDataService<ResourceModel>
    {
        Task<bool> DeleteUnusedResourcesAsync(Expression<Func<ResourceModel, bool>> func);

        Task<ResourceModel> FindIncludesAsync(string id);

        Task<List<Guid>> GetDiaryIdsAsync(string id);
    }
}
