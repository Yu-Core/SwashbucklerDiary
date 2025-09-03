using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Repository
{
    public interface IResourceRepository : IBaseRepository<ResourceModel>
    {
        Task<bool> DeleteUnusedResourcesAsync(Expression<Func<ResourceModel, bool>> expression);

        /// <summary>
        /// Query used resources include privacy mode
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<List<string>> QueryTrulyUsedResourcesAsync(bool privacyMode);

        Task<ResourceModel> FindIncludesAsync(string id);

        Task<List<Guid>> GetDiaryIdsAsync(string id);
    }
}
