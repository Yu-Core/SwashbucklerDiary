using SqlSugar;
using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Repository
{
    public class TagRepository : BaseRepository<TagModel>, ITagRepository
    {
        public TagRepository(ISqlSugarClient context) : base(context)
        {
        }

        public override Task<List<TagModel>> GetListAsync()
        {
            return base.Context.Queryable<TagModel>()
                .OrderByDescending(it => it.Id)
                .ToListAsync();
        }

        public override Task<List<TagModel>> GetListAsync(Expression<Func<TagModel, bool>> whereExpression)
        {
            return base.Context.Queryable<TagModel>()
                .Where(whereExpression)
                .OrderByDescending(it => it.Id)
                .ToListAsync();
        }

        public override Task<bool> InsertAsync(TagModel model)
        {
            model.CreateTime = model.UpdateTime = DateTime.Now;
            return base.InsertAsync(model);
        }

        public override Task<int> InsertReturnIdentityAsync(TagModel model)
        {
            model.CreateTime = model.UpdateTime = DateTime.Now;
            return base.InsertReturnIdentityAsync(model);
        }

        public override Task<TagModel> InsertReturnEntityAsync(TagModel model)
        {
            model.CreateTime = model.UpdateTime = DateTime.Now;
            return base.InsertReturnEntityAsync(model);
        }

        public override Task<bool> UpdateAsync(TagModel model)
        {
            model.UpdateTime = DateTime.Now;
            return base.UpdateAsync(model);
        }

        public override Task<bool> DeleteAsync(TagModel tag)
        {
            return base.Context.DeleteNav(tag)
                .Include(it => it.Diaries, new DeleteNavOptions()
                {
                    ManyToManyIsDeleteA = true
                })
                .ExecuteCommandAsync();
        }

        public Task<TagModel> GetByIdIncludesAsync(dynamic id)
        {
            return Context.Queryable<TagModel>()
                .Includes(it => it.Diaries)
                .InSingleAsync(id);
        }

        public Task<TagModel> GetFirstIncludesAsync(Expression<Func<TagModel, bool>> whereExpression)
        {
            return Context.Queryable<TagModel>()
                .Includes(it => it.Diaries)
                .FirstAsync(whereExpression);
        }
    }
}
