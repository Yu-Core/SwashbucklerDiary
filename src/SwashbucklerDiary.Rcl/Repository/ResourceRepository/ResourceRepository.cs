using SqlSugar;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Repository
{
    public class ResourceRepository : BaseRepository<ResourceModel>, IResourceRepository
    {
        public ResourceRepository(ISqlSugarClient context) : base(context)
        {
        }

        public override async Task<List<ResourceModel>> GetListAsync()
        {
            var resources = await base.Context.Queryable<ResourceModel>().ToListAsync();
            resources.Reverse();
            return resources;
        }

        public override async Task<List<ResourceModel>> GetListAsync(Expression<Func<ResourceModel, bool>> expression)
        {
            var resources = await base.Context.Queryable<ResourceModel>().Where(expression).ToListAsync();
            resources.Reverse();
            return resources;
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
