using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Services
{
    public class DiaryService : BaseDataService<DiaryModel>, IDiaryService
    {
        private readonly IDiaryRepository _iDiaryRepository;

        private readonly ISettingService _settingService;

        public DiaryService(IDiaryRepository iDiaryRepository, ISettingService settingService)
        {
            base._iBaseRepository = iDiaryRepository;
            _iDiaryRepository = iDiaryRepository;
            _settingService = settingService;
        }

        public override Task<List<DiaryModel>> QueryAsync()
        {
            var privacyMode = _settingService.GetTemp(s => s.PrivacyMode);
            return _iBaseRepository.GetListAsync(it => it.Private == privacyMode);
        }

        public override Task<List<DiaryModel>> QueryAsync(Expression<Func<DiaryModel, bool>> expression)
        {
            var privacyMode = _settingService.GetTemp(s => s.PrivacyMode);
            expression = expression.And(it => it.Private == privacyMode);
            return _iBaseRepository.GetListAsync(expression);
        }

        public Task<List<TagModel>> GetTagsAsync(Guid id)
        {
            return _iDiaryRepository.GetTagsAsync(id);
        }

        public Task<bool> UpdateIncludesAsync(DiaryModel model)
        {
            return _iDiaryRepository.UpdateIncludesAsync(model);
        }

        public Task<bool> UpdateIncludesAsync(List<DiaryModel> models)
        {
            return _iDiaryRepository.UpdateIncludesAsync(models);
        }

        public Task<bool> UpdateTagsAsync(DiaryModel model)
        {
            return _iDiaryRepository.UpdateTagsAsync(model);
        }

        public Task<bool> ImportAsync(List<DiaryModel> diaries)
        {
            return _iDiaryRepository.ImportAsync(diaries);
        }
    }
}
