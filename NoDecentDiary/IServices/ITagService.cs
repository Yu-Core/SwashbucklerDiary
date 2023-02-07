using NoDecentDiary.Models;

namespace NoDecentDiary.IServices
{
    public interface ITagService : IBaseService<TagModel>
    {
        Task<List<TagModel>> GetDiaryTagsAsync(int diaryId);
    }
}
