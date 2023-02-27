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

        public override Task<List<DiaryModel>> GetListAsync()
        {
            return base.Context.Queryable<DiaryModel>()
                .OrderByDescending(it => it.Id)
                .ToListAsync();
        }

        public override Task<List<DiaryModel>> GetListAsync(Expression<Func<DiaryModel, bool>> whereExpression)
        {
            return base.Context.Queryable<DiaryModel>()
                .Where(whereExpression)
                .OrderByDescending(it => it.Id)
                .ToListAsync();
        }

        public override Task<List<DiaryModel>> GetListTakeAsync(int count)
        {
            return base.Context.Queryable<DiaryModel>()
                .OrderByDescending(it => it.Id)
                .Take(count)
                .ToListAsync();
        }

        public override Task<List<DiaryModel>> GetListTakeAsync(int count, Expression<Func<DiaryModel, bool>> func)
        {
            return base.Context.Queryable<DiaryModel>()
                .Where(func)
                .OrderByDescending(it => it.Id)
                .Take(count)
                .ToListAsync();
        }

        public override Task<bool> InsertAsync(DiaryModel model)
        {
            model.CreateTime = DateTime.Now;
            model.UpdateTime = DateTime.Now;
            return base.Context.InsertNav(model)
            .Include(it => it.Tags)
            .ExecuteCommandAsync();
        }

        public override Task<int> InsertReturnIdentityAsync(DiaryModel model)
        {
            model.CreateTime = model.UpdateTime = DateTime.Now;
            return base.InsertReturnIdentityAsync(model);
        }

        public override Task<DiaryModel> InsertReturnEntityAsync(DiaryModel model)
        {
            model.CreateTime = model.UpdateTime = DateTime.Now;
            return base.InsertReturnEntityAsync(model);
        }

        public override Task<bool> UpdateAsync(DiaryModel model)
        {
            model.UpdateTime = DateTime.Now;
            return base.UpdateAsync(model);
        }

        public override Task<bool> DeleteAsync(DiaryModel model)
        {
            return base.Context.DeleteNav(model)
                .Include(it => it.Tags, new DeleteNavOptions()
                {
                    ManyToManyIsDeleteA = true
                })
                .ExecuteCommandAsync();
        }

        public Task<DiaryModel> GetByIdIncludesAsync(int id)
        {
            return Context.Queryable<DiaryModel>()
                .Includes(it => it.Tags)
                .InSingleAsync(id);
        }

        public Task<DiaryModel> GetFirstIncludesAsync(Expression<Func<DiaryModel, bool>> whereExpression)
        {
            return Context.Queryable<DiaryModel>()
                .Includes(it => it.Tags)
                .FirstAsync(whereExpression);
        }

        public Task<List<TagModel>> GetTagsAsync(int id)
        {
            return base.Context.Queryable<DiaryModel>()
                .LeftJoin<DiaryTagModel>((d, dt) => d.Id == dt.DiaryId)
                .LeftJoin<TagModel>((d, dt, t) => dt.TagId == t.Id)
                .Where(d => d.Id == id)
                .Select((d, dt, t) => t)
                .ToListAsync();
        }

        public Task<List<DiaryModel>> GetListIncludesAsync()
        {
            return base.Context.Queryable<DiaryModel>()
                .Includes(it => it.Tags)
                .OrderByDescending(it => it.Id)
                .ToListAsync();
        }

        public Task<List<DiaryModel>> GetListIncludesAsync(Expression<Func<DiaryModel, bool>> func)
        {
            return base.Context.Queryable<DiaryModel>()
                .Includes(it => it.Tags)
                .Where(func)
                .OrderByDescending(it => it.Id)
                .ToListAsync();
        }

        public Task<bool> UpdateIncludesAsync(DiaryModel model)
        {
            model.UpdateTime = DateTime.Now;
            return base.Context.UpdateNav(model)
            .Include(it => it.Tags, new UpdateNavOptions
            {
                ManyToManyIsUpdateA = true
            })
            .ExecuteCommandAsync();
        }

        public Task<bool> UpdateTagsAsync(DiaryModel model)
        {
            model.UpdateTime = DateTime.Now;
            return base.Context.UpdateNav(model)
            .Include(it => it.Tags)
            .ExecuteCommandAsync();
        }
    }
}
