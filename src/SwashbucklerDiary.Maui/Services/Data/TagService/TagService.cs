using SwashbucklerDiary.Maui.IRepository;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Maui.Services
{
    public class TagService : BaseDataService<TagModel>, ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            base._iBaseRepository = tagRepository;
            _tagRepository = tagRepository;
        }

        public Task<TagModel> FindIncludesAsync(Guid id)
        {
            return _tagRepository.GetByIdIncludesAsync(id);
        }

        public Task<TagModel> FindIncludesAsync(Expression<Func<TagModel, bool>> expression)
        {
            return _tagRepository.GetFirstIncludesAsync(expression);
        }
    }
}
