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

        public Task<ResourceModel> FindIncludesAsync(string id)
        {
            return Context.Queryable<ResourceModel>()
                        .Includes(r => r.Diaries, d => d.Tags)
                        .Includes(r => r.Diaries, d => d.Resources)
                        .InSingleAsync(id);
        }

        public Task<List<Guid>> GetDiaryIdsAsync(string id)
        {
            return Context.Queryable<DiaryResourceModel>()
                .Where(it => it.ResourceUri == id)
                .Select(it => it.DiaryId)
                .Distinct()
                .ToListAsync();
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

        public async Task<(List<string?> currentUnusedResourceUris, List<string?> trulyUnusedResourceUris)> QueryUnusedResourcesAsync(Expression<Func<ResourceModel, bool>> expression, bool privacyMode)
        {
            Expression<Func<ResourceModel, bool>> expression2 = r => true;
            expression2 = expression2.And(expression);

            var db = Context.AsTenant().GetConnection("0");
            var privacyDb = Context.AsTenant().GetConnection("1");
            var (currentDb, notCurrentDb) = privacyMode ? (privacyDb, db) : (db, privacyDb);
            var resourceUris = await currentDb.Queryable<ResourceModel>()
                .LeftJoin<DiaryResourceModel>((r, dr) => r.ResourceUri == dr.ResourceUri)
                .Where(expression2)
                .Where((r, dr) => dr.ResourceUri == null) // 只选择没有匹配的记录
                .Select(r => r.ResourceUri)
                .ToListAsync();
            var resourceUris2 = await notCurrentDb.Queryable<ResourceModel>()
                .LeftJoin<DiaryResourceModel>((r, dr) => r.ResourceUri == dr.ResourceUri)
                .Where(expression2)
                .Where((r, dr) => dr.ResourceUri == null) // 只选择没有匹配的记录
                .Select(r => r.ResourceUri)
                .ToListAsync();

            var trulyUnusedResources = resourceUris.Intersect(resourceUris2).ToList();
            return (resourceUris, trulyUnusedResources);
        }
    }
}
