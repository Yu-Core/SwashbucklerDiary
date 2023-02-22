using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.IServices
{
    public interface IDiaryService : IBaseService<DiaryModel>
    {
        Task<DiaryModel> FindIncludesAsync(int id);
        Task<DiaryModel> FindIncludesAsync(Expression<Func<DiaryModel, bool>> func);
        Task<List<DiaryModel>> QueryIncludesAsync();
        Task<List<DiaryModel>> QueryIncludesAsync(Expression<Func<DiaryModel, bool>> func);
        Task<bool> UpdateTagsAsync(DiaryModel model);
        Task<List<TagModel>> GetTagsAsync(int id);
        Task<bool> UpdateIncludesAsync(DiaryModel model);
    }
}
