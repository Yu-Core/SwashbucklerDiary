using NoDecentDiary.Models;

namespace NoDecentDiary.IServices
{
    public interface IDiaryService : IBaseService<DiaryModel>
    {
        Task<List<DiaryModel>> GetDiariesByTagAsync(int tagId);
    }
}
