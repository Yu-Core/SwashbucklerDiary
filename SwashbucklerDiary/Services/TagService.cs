using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Services
{
    public class TagService : BaseService<TagModel>, ITagService
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
