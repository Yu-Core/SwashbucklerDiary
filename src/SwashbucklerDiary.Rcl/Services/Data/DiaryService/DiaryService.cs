using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Services
{
    public class DiaryService : BaseDataService<DiaryModel>, IDiaryService
    {
        private readonly IDiaryRepository _iDiaryRepository;

        public DiaryService(IDiaryRepository iDiaryRepository)
        {
            base._iBaseRepository = iDiaryRepository;
            _iDiaryRepository = iDiaryRepository;
        }

        public override Task<List<DiaryModel>> QueryAsync()
        {
            return _iBaseRepository.GetListAsync();
        }

        public override Task<List<DiaryModel>> QueryAsync(Expression<Func<DiaryModel, bool>> expression)
        {
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

        public Task<bool> MovePrivacyDiaryAsync(DiaryModel diary, bool toPrivacyMode)
        {
            return _iDiaryRepository.MovePrivacyDiaryAsync(diary, toPrivacyMode);
        }

        public Task<bool> MovePrivacyDiariesAsync()
        {
            return _iDiaryRepository.MovePrivacyDiariesAsync();
        }
    }
}
