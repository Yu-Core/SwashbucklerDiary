using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.IServices
{
    public interface IAchievementService
    {
        Task<List<string>> UpdateUserState(AchievementType type);
        Task<List<string>> UpdateUserState(AchievementType type, int count);
        Task<List<AchievementModel>> GetAchievements();
    }
}
