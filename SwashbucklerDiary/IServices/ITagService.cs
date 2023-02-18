using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.IServices
{
    public interface ITagService : IBaseService<TagModel>
    {
        Task<List<TagModel>> GetDiaryTagsAsync(int diaryId);
    }
}
