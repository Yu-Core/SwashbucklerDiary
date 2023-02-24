using SwashbucklerDiary.Models.Data;

namespace SwashbucklerDiary.IServices
{
    public interface IAchievementService
    {
        Task<List<string>> UpdateUserState(AchievementType type);
        Task<List<string>> UpdateUserState(UserStateModel userStateModel);
    }
}
