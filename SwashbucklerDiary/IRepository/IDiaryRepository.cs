using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.IRepository
{
    public interface IDiaryRepository : IBaseRepository<DiaryModel>
    {
        Task<bool> UpdateTagsAsync(DiaryModel model);
        Task<DiaryModel> GetByIdIncludesAsync(int id);
        Task<DiaryModel> GetFirstIncludesAsync(Expression<Func<DiaryModel, bool>> whereExpression);
        Task<List<TagModel>> GetTagsAsync(int id);
        Task<List<DiaryModel>> GetListIncludesAsync();
        Task<List<DiaryModel>> GetListIncludesAsync(Expression<Func<DiaryModel, bool>> func);
        Task<bool> UpdateIncludesAsync(DiaryModel model);
    }
}
