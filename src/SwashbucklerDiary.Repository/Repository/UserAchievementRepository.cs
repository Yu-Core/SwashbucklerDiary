using SqlSugar;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Repository
{
    public class UserAchievementRepository : BaseRepository<UserAchievementModel>, IUserAchievementRepository
    {
        public UserAchievementRepository(ISqlSugarClient context) : base(context)
        {
        }
    }
}
