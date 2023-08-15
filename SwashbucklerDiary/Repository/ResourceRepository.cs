using SqlSugar;
using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Repository
{
    public class ResourceRepository : BaseRepository<ResourceModel>, IResourceRepository
    {
        public ResourceRepository(ISqlSugarClient context) : base(context)
        {
        }

        public async Task<List<ResourceModel>> DeleteUnusedResourcesAsync(Expression<Func<ResourceModel, bool>> func)
        {
            var resources = await base.Context.Queryable<ResourceModel>()
                .Where(func)
                .Where(p => SqlFunc.Subqueryable<DiaryResourceModel>().Where(s => s.ResourceUri == p.ResourceUri).NotAny())
                .ToListAsync();
            await base.Context.Deleteable<ResourceModel>(resources).ExecuteCommandAsync();
            return resources;
        }
    }
}
