using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.IServices
{
    public interface IDiaryService : IBaseService<DiaryModel>
    {
        Task<bool> UpdateTagsAsync(DiaryModel model);

        Task<List<TagModel>> GetTagsAsync(Guid id);

        Task<bool> UpdateIncludesAsync(DiaryModel model);

        Task<bool> UpdateIncludesAsync(List<DiaryModel> models);

        Task<int> GetWordCount(WordCountType type);

        int GetWordCount(List<DiaryModel> diaries, WordCountType type);

        Task<bool> ImportAsync(List<DiaryModel> diaries);

        Task<List<DateOnly>> GetAllDates();

        Task<List<DateOnly>> GetAllDates(Expression<Func<DiaryModel, bool>> func);
    }
}
