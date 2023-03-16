using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.IServices
{
    public interface IDiaryService : IBaseService<DiaryModel>
    {
        Task<DiaryModel> FindIncludesAsync(Guid id);
        Task<DiaryModel> FindIncludesAsync(Expression<Func<DiaryModel, bool>> func);
        Task<List<DiaryModel>> QueryIncludesAsync();
        Task<List<DiaryModel>> QueryIncludesAsync(Expression<Func<DiaryModel, bool>> func);
        Task<bool> UpdateTagsAsync(DiaryModel model);
        Task<List<TagModel>> GetTagsAsync(Guid id);
        Task<bool> UpdateIncludesAsync(DiaryModel model);
        Task<int> GetWordCount(WordCountType type);
        Task<bool> ImportAsync(List<DiaryModel> diaries);
    }
}
