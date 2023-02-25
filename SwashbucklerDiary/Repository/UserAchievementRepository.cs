using SqlSugar;
using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Repository
{
    public class UserAchievementRepository : BaseRepository<UserAchievementModel>, IUserAchievementRepository
    {
        public UserAchievementRepository(ISqlSugarClient context) : base(context)
        {
        }
    }
}
