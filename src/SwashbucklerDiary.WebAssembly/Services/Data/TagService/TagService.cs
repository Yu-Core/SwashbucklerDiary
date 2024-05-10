using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.WebAssembly.Services
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
    }
}
