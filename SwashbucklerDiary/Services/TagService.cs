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

        public Task<TagModel> FindIncludesAsync(int id)
        {
            return _tagRepository.GetByIdIncludesAsync(id);
        }

        public Task<TagModel> FindIncludesAsync(Expression<Func<TagModel, bool>> func)
        {
            return _tagRepository.GetByIdIncludesAsync(func);
        }
    }
}
