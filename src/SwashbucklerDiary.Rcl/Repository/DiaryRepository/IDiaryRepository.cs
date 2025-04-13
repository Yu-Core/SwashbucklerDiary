using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Repository
{
    public interface IDiaryRepository : IBaseRepository<DiaryModel>
    {
        Task<bool> UpdateTagsAsync(DiaryModel model);

        Task<List<TagModel>> GetTagsAsync(Guid id);

        Task<bool> UpdateIncludesAsync(DiaryModel model);

        Task<bool> UpdateIncludesAsync(List<DiaryModel> models);

        Task<bool> ImportAsync(List<DiaryModel> diaries);

        Task<bool> MovePrivacyDiaryAsync(DiaryModel diary, bool toPrivacyMode);

        Task<bool> MovePrivacyDiariesAsync();

        Task<DiaryModel> GetFirstTemplateAsync(Expression<Func<DiaryModel, bool>> whereExpression);

        Task<List<DiaryModel>> GetTemplateListAsync();

        Task<List<DiaryModel>> GetTemplateListAsync(Expression<Func<DiaryModel, bool>> whereExpression);
    }
}
