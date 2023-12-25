using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Maui.IRepository
{
    public interface IResourceRepository : IBaseRepository<ResourceModel>
    {
        Task<List<ResourceModel>> QueryUnusedResourcesAsync(Expression<Func<ResourceModel, bool>> expression);
    }
}
