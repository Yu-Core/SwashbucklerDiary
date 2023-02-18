using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.IServices
{
    public interface IDiaryTagService : IBaseService<DiaryTagModel>
    {
        Task<bool> AddTagsAsync(int diaryId, List<TagModel> tagModels);
    }
}
