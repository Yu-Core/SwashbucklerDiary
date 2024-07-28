using SqlSugar;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Repository
{
    public class DiaryRepository : BaseRepository<DiaryModel>, IDiaryRepository
    {
        public DiaryRepository(ISqlSugarClient context) : base(context)
        {
        }

        public override Task<bool> InsertAsync(DiaryModel model)
        {
            return base.Context.InsertNav(model)
            .Include(it => it.Tags)
            .Include(it => it.Resources)
            .ExecuteCommandAsync();
        }

        public override Task<bool> DeleteAsync(DiaryModel model)
        {
            return base.Context.DeleteNav(model)
                .Include(it => it.Tags, new DeleteNavOptions()
                {
                    ManyToManyIsDeleteA = true
                })
                .Include(it => it.Resources, new DeleteNavOptions()
                {
                    ManyToManyIsDeleteA = true
                })
                .ExecuteCommandAsync();
        }

        public override Task<DiaryModel> GetByIdAsync(dynamic id)
        {
            return Context.Queryable<DiaryModel>()
                .Includes(it => it.Tags)
                .Includes(it => it.Resources)
                .InSingleAsync(id);
        }

        public override Task<DiaryModel> GetFirstAsync(Expression<Func<DiaryModel, bool>> expression)
        {
            return Context.Queryable<DiaryModel>()
                .Includes(it => it.Tags)
                .Includes(it => it.Resources)
                .FirstAsync(expression);
        }

        public async Task<List<TagModel>> GetTagsAsync(Guid id)
        {
            return await base.Context.Queryable<DiaryTagModel>()
                .Where(dt => dt.DiaryId == id)
                .LeftJoin<TagModel>((dt, t) => dt.TagId == t.Id)
                .Select((dt, t) => t)
                .ToListAsync();
        }

        public override Task<List<DiaryModel>> GetListAsync()
        {
            return base.Context.Queryable<DiaryModel>()
                .Includes(it => it.Tags)
                .Includes(it => it.Resources)
                .OrderByDescending(it => it.CreateTime)
                .ToListAsync();
        }

        public override Task<List<DiaryModel>> GetListAsync(Expression<Func<DiaryModel, bool>> expression)
        {
            return base.Context.Queryable<DiaryModel>()
                .Includes(it => it.Tags)
                .Includes(it => it.Resources)
                .Where(expression)
                .OrderByDescending(it => it.CreateTime)
                .ToListAsync();
        }

        public Task<bool> UpdateIncludesAsync(DiaryModel model)
        {
            return base.Context.UpdateNav(model)
            .Include(it => it.Tags, new UpdateNavOptions
            {
                ManyToManyIsUpdateA = true
            })
            .Include(it => it.Resources, new UpdateNavOptions
            {
                ManyToManyIsUpdateA = true,
                ManyToManyIsUpdateB = true,
            })
            .ExecuteCommandAsync();
        }

        public Task<bool> UpdateIncludesAsync(List<DiaryModel> models)
        {
            return base.Context.UpdateNav(models)
            .Include(it => it.Tags, new UpdateNavOptions
            {
                ManyToManyIsUpdateA = true
            })
            .Include(it => it.Resources, new UpdateNavOptions
            {
                ManyToManyIsUpdateA = true,
                ManyToManyIsUpdateB = true,
            })
            .ExecuteCommandAsync();
        }

        public Task<bool> UpdateTagsAsync(DiaryModel model)
        {
            return base.Context.UpdateNav(model)
            .Include(it => it.Tags)
            .ExecuteCommandAsync();
        }

        public Task<bool> ImportAsync(List<DiaryModel> diaries)
        {
            return base.Context.UpdateNav(diaries, new UpdateNavRootOptions()
            {
                IsInsertRoot = true
            })
            .Include(it => it.Tags, new UpdateNavOptions
            {
                ManyToManyIsUpdateA = true,
                ManyToManyIsUpdateB = true
            })
            .Include(it => it.Resources, new UpdateNavOptions
            {
                ManyToManyIsUpdateA = true,
                ManyToManyIsUpdateB = true
            })
            .ExecuteCommandAsync();
        }
    }
}
