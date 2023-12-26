using SqlSugar;
using SwashbucklerDiary.Maui.IRepository;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Maui.Repository
{
    public class ResourceRepository : BaseRepository<ResourceModel>, IResourceRepository
    {
        public ResourceRepository(ISqlSugarClient context) : base(context)
        {
        }

        public Task<List<ResourceModel>> QueryUnusedResourcesAsync(Expression<Func<ResourceModel, bool>> expression)
        {
            return base.Context.Queryable<ResourceModel>()
                .Where(expression)
                .Where(p => SqlFunc.Subqueryable<DiaryResourceModel>().Where(s => s.ResourceUri == p.ResourceUri).NotAny())
                .ToListAsync();
        }
    }
}
