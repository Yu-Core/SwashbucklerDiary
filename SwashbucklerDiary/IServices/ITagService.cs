using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.IServices
{
    public interface ITagService : IBaseService<TagModel>
    {
        Task<TagModel> FindIncludesAsync(Guid id);

        Task<TagModel> FindIncludesAsync(Expression<Func<TagModel, bool>> func);
    }
}
