using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.IServices
{
    public interface IDiaryService : IBaseService<DiaryModel>
    {
        Task<List<DiaryModel>> GetDiariesByTagAsync(int tagId);
    }
}
