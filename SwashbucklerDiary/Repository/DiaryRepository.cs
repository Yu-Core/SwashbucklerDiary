using SqlSugar;
using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Repository
{
    public class DiaryRepository : BaseRepository<DiaryModel>, IDiaryRepository
    {
        public DiaryRepository(ISqlSugarClient context) : base(context)
        {
        }

        public override Task<List<DiaryModel>> GetListTakeAsync(int count)
        {
            return base.Context.Queryable<DiaryModel>()
                .Includes(it => it.Tags)
                .Includes(it => it.Resources)
                .OrderByDescending(it => it.CreateTime)
                .Take(count)
                .ToListAsync();
        }

        public override Task<List<DiaryModel>> GetListTakeAsync(int count, Expression<Func<DiaryModel, bool>> func)
        {
            return base.Context.Queryable<DiaryModel>()
                .Includes(it => it.Tags)
                .Includes(it => it.Resources)
                .Where(func)
                .OrderByDescending(it => it.CreateTime)
                .Take(count)
                .ToListAsync();
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

        public override Task<DiaryModel> GetFirstAsync(Expression<Func<DiaryModel, bool>> whereExpression)
        {
            return Context.Queryable<DiaryModel>()
                .Includes(it => it.Tags)
                .Includes(it => it.Resources)
                .FirstAsync(whereExpression);
        }

        public Task<List<TagModel>> GetTagsAsync(Guid id)
        {
            return base.Context.Queryable<DiaryModel>()
                .LeftJoin<DiaryTagModel>((d, dt) => d.Id == dt.DiaryId)
                .LeftJoin<TagModel>((d, dt, t) => dt.TagId == t.Id)
                .Where(d => d.Id == id)
                .Select((d, dt, t) => t)
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

        public override Task<List<DiaryModel>> GetListAsync(Expression<Func<DiaryModel, bool>> func)
        {
            return base.Context.Queryable<DiaryModel>()
                .Includes(it => it.Tags)
                .Includes(it => it.Resources)
                .Where(func)
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
                ManyToManyIsUpdateA = true
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

        public Task<List<DateOnly>> GetAllDates()
        {
            return GetAllDates(it=>true);
        }

        public async Task<List<DateOnly>> GetAllDates(Expression<Func<DiaryModel, bool>> func)
        {
            var dates = await base.Context.Queryable<DiaryModel>()
                .Where(func)
                .Select(s => s.CreateTime.Date)
                .Distinct()
                .ToListAsync();

            return dates.Select(DateOnly.FromDateTime).ToList();
        }
    }
}
