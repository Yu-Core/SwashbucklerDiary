using SqlSugar;
using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwashbucklerDiary.Repository
{
    public class UserAchievementRepository : BaseRepository<UserAchievementModel>, IUserAchievementRepository
    {
        public UserAchievementRepository(ISqlSugarClient context) : base(context)
        {
        }
    }
}
