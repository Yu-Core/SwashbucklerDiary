using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface ITagService : IBaseDataService<TagModel>
    {
        Task<TagModel> FindIncludesAsync(Guid id);

        Task<TagModel> FindIncludesAsync(Expression<Func<TagModel, bool>> func);
    }
}
