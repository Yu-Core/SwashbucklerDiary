using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Repository
{
    public interface ITagRepository : IBaseRepository<TagModel>
    {
        Task<TagModel> GetByIdIncludesAsync(dynamic id);

        Task<TagModel> GetFirstIncludesAsync(Expression<Func<TagModel, bool>> expression);
    }
}
