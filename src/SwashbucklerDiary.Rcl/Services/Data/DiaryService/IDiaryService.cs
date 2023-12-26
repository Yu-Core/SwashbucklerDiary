using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface IDiaryService : IBaseDataService<DiaryModel>
    {
        Task<bool> UpdateTagsAsync(DiaryModel model);

        Task<List<TagModel>> GetTagsAsync(Guid id);

        Task<bool> UpdateIncludesAsync(DiaryModel model);

        Task<bool> UpdateIncludesAsync(List<DiaryModel> models);

        Task<int> GetWordCount(WordCountStatistics statistics);

        int GetWordCount(List<DiaryModel> diaries, WordCountStatistics statistics);

        Task<bool> ImportAsync(List<DiaryModel> diaries);

        Task<List<DateOnly>> GetAllDates();

        Task<List<DateOnly>> GetAllDates(Expression<Func<DiaryModel, bool>> func);
    }
}
