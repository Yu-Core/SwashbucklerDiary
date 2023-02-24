using SwashbucklerDiary.Models.Data;

namespace SwashbucklerDiary.IRepository
{
    public interface IUserStateModelRepository : IBaseRepository<UserStateModel>
    {
        Task<UserStateModel> InsertOrUpdateAsync(AchievementType type);
        new Task<UserStateModel> InsertOrUpdateAsync(UserStateModel userState);
    }
}
