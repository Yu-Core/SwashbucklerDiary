using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public class TagService : BaseDataService<TagModel>, ITagService
    {
        private readonly ITagRepository _tagRepository;

        private readonly ISettingService _settingService;

        public TagService(ITagRepository tagRepository, ISettingService settingService)
        {
            base._iBaseRepository = tagRepository;
            _tagRepository = tagRepository;
            _settingService = settingService;
        }

        public Task<TagModel> FindIncludesAsync(Guid id)
        {
            var privacyMode = _settingService.GetTemp<bool>(TempSetting.PrivacyMode);
            return _tagRepository.GetByIdIncludesAsync(id, it => it.Diaries!
                                  .Where(d => d.Private == privacyMode)
                                  .OrderByDescending(it => it.CreateTime)
                                  .ToList());
        }

        public Task<Dictionary<Guid, int>> TagsDiaryCount()
        {
            return _tagRepository.TagsDiaryCount();
        }
    }
}
