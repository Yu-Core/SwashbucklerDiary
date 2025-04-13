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

        Task<bool> ImportAsync(List<DiaryModel> diaries);

        Task<bool> MovePrivacyDiaryAsync(DiaryModel diary, bool toPrivacyMode);

        Task<bool> MovePrivacyDiariesAsync();

        Task<DiaryModel> FindTemplateAsync(Expression<Func<DiaryModel, bool>> expression);

        Task<List<DiaryModel>> QueryTemplateAsync();

        Task<List<DiaryModel>> QueryTemplateAsync(Expression<Func<DiaryModel, bool>> expression);
    }
}
