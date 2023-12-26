using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface IAchievementService
    {
        Task<List<string>> UpdateUserState(Achievement achievement);

        Task<List<string>> UpdateUserState(Achievement achievement, int count);

        Task<List<AchievementModel>> GetAchievements();
    }
}
