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

        public Task<TagModel> GetByIdIncludesAsync(dynamic id)
        {
            return Context.Queryable<TagModel>()
                .Includes(it => it.Diaries!
                                  .Where(d => !d.Private)
                                  .OrderByDescending(it => it.CreateTime)
                                  .ToList(),
                          d => d.Tags)
                .Includes(it => it.Diaries!
                                  .Where(d => !d.Private)
                                  .OrderByDescending(it => it.CreateTime)
                                  .ToList(),
                          d => d.Resources)
                .InSingleAsync(id);
        }

        public Task<TagModel> GetFirstIncludesAsync(Expression<Func<TagModel, bool>> expression)
        {
            return Context.Queryable<TagModel>()
                .Includes(it => it.Diaries!
                                  .Where(d => !d.Private)
                                  .OrderByDescending(it => it.CreateTime)
                                  .ToList(),
                          d => d.Tags)
                .Includes(it => it.Diaries!
                                  .Where(d => !d.Private)
                                  .OrderByDescending(it => it.CreateTime)
                                  .ToList(),
                          d => d.Resources)
                .FirstAsync(expression);
        }
    }
}
