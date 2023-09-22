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

        public override Task<DiaryModel> FindAsync(Guid id)
        {
            return _iDiaryRepository.GetByIdAsync(id);
        }

        public override Task<DiaryModel> FindAsync(Expression<Func<DiaryModel, bool>> func)
        {
            return _iDiaryRepository.GetFirstAsync(func);
        }

        public override Task<List<DiaryModel>> QueryAsync()
        {
            return _iDiaryRepository.GetListAsync();
        }

        public override Task<List<DiaryModel>> QueryAsync(Expression<Func<DiaryModel, bool>> func)
        {
            return _iDiaryRepository.GetListAsync(func);
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

        public Task<bool> ImportAsync(List<DiaryModel> diaries)
        {
            return _iDiaryRepository.ImportAsync(diaries);
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

        public Task<List<DateOnly>> GetAllDates()
        {
            return _iDiaryRepository.GetAllDates();
        }

        public Task<List<DateOnly>> GetAllDates(Expression<Func<DiaryModel, bool>> func)
        {
            return _iDiaryRepository.GetAllDates(func);
        }
    }
}
