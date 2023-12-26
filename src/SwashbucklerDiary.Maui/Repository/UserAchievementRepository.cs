using SqlSugar;
using SwashbucklerDiary.Maui.IRepository;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Repository
{
    public class UserAchievementRepository : BaseRepository<UserAchievementModel>, IUserAchievementRepository
    {
        public UserAchievementRepository(ISqlSugarClient context) : base(context)
        {
        }
    }
}
