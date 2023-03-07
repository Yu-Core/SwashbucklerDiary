using SwashbucklerDiary.Extend;
using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Services
{
    public class DiaryService : BaseService<DiaryModel>, IDiaryService
    {
        private readonly IDiaryRepository _iDiaryRepository;

        public DiaryService(IDiaryRepository iDiaryRepository)
        {
            base._iBaseRepository = iDiaryRepository;
            _iDiaryRepository = iDiaryRepository;
        }

        public Task<DiaryModel> FindIncludesAsync(Guid id)
        {
            return _iDiaryRepository.GetByIdIncludesAsync(id);
        }

        public Task<DiaryModel> FindIncludesAsync(Expression<Func<DiaryModel, bool>> func)
        {
            return _iDiaryRepository.GetFirstIncludesAsync(func);
        }

        public Task<List<DiaryModel>> QueryIncludesAsync()
        {
            return _iDiaryRepository.GetListIncludesAsync();
        }

        public Task<List<DiaryModel>> QueryIncludesAsync(Expression<Func<DiaryModel, bool>> func)
        {
            return _iDiaryRepository.GetListIncludesAsync(func);
        }

        public Task<List<TagModel>> GetTagsAsync(Guid id)
        {
            return _iDiaryRepository.GetTagsAsync(id);
        }

        public Task<bool> UpdateIncludesAsync(DiaryModel model)
        {
            return _iDiaryRepository.UpdateIncludesAsync(model);
        }

        public Task<bool> UpdateTagsAsync(DiaryModel model)
        {
            return _iDiaryRepository.UpdateTagsAsync(model);
        }

        public async Task<int> GetWordCount(WordCountType type)
        {
            var diaries = await QueryAsync(it=>!it.Private);
            var wordCount = 0;
            if (type == WordCountType.Word)
            {
                foreach (var item in diaries)
                {
                    wordCount += item.Content?.WordCount() ?? 0;
                }
            }
            if(type == WordCountType.Character)
            {
                foreach (var item in diaries)
                {
                    wordCount += item.Content?.CharacterCount() ?? 0;
                }
            }
            return wordCount;
        }
    }
}
