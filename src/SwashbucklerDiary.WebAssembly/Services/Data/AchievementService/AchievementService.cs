using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class AchievementService : Rcl.Services.AchievementService
    {
        public AchievementService(IUserAchievementRepository userAchievementRepository,
            IUserStateModelRepository userStateModelRepository,
            [FromKeyedServices("Achievements")] Dictionary<Achievement, int[]> defaultAchievements)
            : base(userAchievementRepository, userStateModelRepository)
        {
            foreach (var item in defaultAchievements)
            {
                foreach (var count in item.Value)
                {
                    _achievements.Add(new(item.Key, count));
                }
            }
        }
    }
}
