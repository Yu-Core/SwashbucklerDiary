using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Repository
{
    public interface IResourceRepository : IBaseRepository<ResourceModel>
    {
        Task<(List<string?> currentUnusedResourceUris, List<string?> trulyUnusedResourceUris)> QueryUnusedResourcesAsync(Expression<Func<ResourceModel, bool>> expression, bool privacyMode);

        Task<ResourceModel> FindIncludesAsync(string id);

        Task<List<Guid>> GetDiaryIdsAsync(string id);
    }
}
