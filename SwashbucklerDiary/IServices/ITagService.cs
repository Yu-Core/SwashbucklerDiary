using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.IServices
{
    public interface ITagService : IBaseService<TagModel>
    {
        Task<TagModel> FindIncludesAsync(int id);
        Task<TagModel> FindIncludesAsync(Expression<Func<TagModel, bool>> func);
    }
}
