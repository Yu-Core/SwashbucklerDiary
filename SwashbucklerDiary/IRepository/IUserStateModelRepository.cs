using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.IRepository
{
    public interface IUserStateModelRepository : IBaseRepository<UserStateModel>
    {
        Task<UserStateModel> InsertOrUpdateAsync(AchievementType type);
        Task<UserStateModel> InsertOrUpdateAsync(AchievementType type, int count);
    }
}
