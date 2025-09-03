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
            var resources = await base.Context.Queryable<ResourceModel>().ToListAsync().ConfigureAwait(false);
            resources.Reverse();
            return resources;
        }

        public override async Task<List<ResourceModel>> GetListAsync(Expression<Func<ResourceModel, bool>> expression)
        {
            var resources = await base.Context.Queryable<ResourceModel>().Where(expression).ToListAsync().ConfigureAwait(false); ;
            resources.Reverse();
            return resources;
        }

        public async Task<bool> DeleteUnusedResourcesAsync(Expression<Func<ResourceModel, bool>>? expression)
        {
            expression ??= r => true;

            var resourceUris = await base.Context.Queryable<ResourceModel>()
                .LeftJoin<DiaryResourceModel>((r, dr) => r.ResourceUri == dr.ResourceUri)
                .Where(expression)
                .Where((r, dr) => dr.ResourceUri == null) // 只选择没有匹配的记录
                .Select(r => r.ResourceUri)
                .ToArrayAsync()
                .ConfigureAwait(false);

            return await DeleteByIdsAsync(resourceUris).ConfigureAwait(false);
        }

        public async Task<List<string>> QueryTrulyUsedResourcesAsync(bool privacyMode)
        {
            var db = Context.AsTenant().GetConnection("0");
            var privacyDb = Context.AsTenant().GetConnection("1");
            var (currentDb, notCurrentDb) = privacyMode ? (privacyDb, db) : (db, privacyDb);

            var resourceUris = await currentDb.Queryable<ResourceModel>()
                .InnerJoin<DiaryResourceModel>((r, dr) => r.ResourceUri == dr.ResourceUri)
                .Select(r => r.ResourceUri)
                .Distinct()
                .ToListAsync()
                .ConfigureAwait(false);
            var resourceUris2 = await notCurrentDb.Queryable<ResourceModel>()
                .Select(r => r.ResourceUri)
                .ToListAsync()
                .ConfigureAwait(false);

            return resourceUris.Union(resourceUris2).ToList();
        }
    }
}
