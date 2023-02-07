using NoDecentDiary.Models;

namespace NoDecentDiary.IServices
{
    public interface IDiaryTagService : IBaseService<DiaryTagModel>
    {
        Task<bool> AddTagsAsync(int diaryId, List<TagModel> tagModels);
    }
}
