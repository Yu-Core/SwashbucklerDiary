using DocumentFormat.OpenXml.EMMA;
using SwashbucklerDiary.Extensions;
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

        public async Task<int> GetWordCount(WordCountType type)
        {
            var diaries = await QueryAsync(it => !it.Private);
            return GetWordCount(diaries, type);
        }

        public int GetWordCount(List<DiaryModel> diaries, WordCountType type)
        {
            return type switch
            {
                WordCountType.Word => diaries.Sum(d => d.Content?.WordCount() ?? 0),
                WordCountType.Character => diaries.Sum(d => d.Content?.CharacterCount() ?? 0),
                _ => default
            };
        }

        public Task<List<DateOnly>> GetAllDates()
        {
            return _iDiaryRepository.GetAllDates();
        }

        public Task<List<DateOnly>> GetAllDates(Expression<Func<DiaryModel, bool>> expression)
        {
            return _iDiaryRepository.GetAllDates(expression);
        }
    }
}
