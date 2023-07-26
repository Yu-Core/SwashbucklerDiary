using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.IRepository
{
    public interface IDiaryRepository : IBaseRepository<DiaryModel>
    {
        Task<bool> UpdateTagsAsync(DiaryModel model);
        Task<List<TagModel>> GetTagsAsync(Guid id);
        Task<bool> UpdateIncludesAsync(DiaryModel model);
        Task<bool> ImportAsync(List<DiaryModel> diaries);
        Task<List<DateOnly>> GetAllDates();
        Task<List<DateOnly>> GetAllDates(Expression<Func<DiaryModel, bool>> func);
    }
}
