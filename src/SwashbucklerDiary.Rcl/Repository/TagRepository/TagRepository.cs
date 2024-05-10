using SqlSugar;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Repository
{
    public class TagRepository : BaseRepository<TagModel>, ITagRepository
    {
        public TagRepository(ISqlSugarClient context) : base(context)
        {
        }

        public override Task<List<TagModel>> GetListAsync()
        {
            return base.Context.Queryable<TagModel>()
                .OrderByDescending(it => it.CreateTime)
                .ToListAsync();
        }

        public override Task<List<TagModel>> GetListAsync(Expression<Func<TagModel, bool>> expression)
        {
            return base.Context.Queryable<TagModel>()
                .Where(expression)
                .OrderByDescending(it => it.CreateTime)
                .ToListAsync();
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

        public Task<TagModel> GetByIdIncludesAsync(dynamic id, Expression<Func<TagModel, List<DiaryModel>>> expression)
        {
            return Context.Queryable<TagModel>()
                .Includes(expression, d => d.Tags)
                .Includes(expression, d => d.Resources)
                .InSingleAsync(id);
        }
    }
}
