using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Services
{
    public class AchievementService : Rcl.Services.AchievementService
    {
        public AchievementService(IUserAchievementRepository userAchievementRepository,
            IUserStateModelRepository userStateModelRepository,
            IStaticWebAssets staticWebAssets)
            : base(userAchievementRepository, userStateModelRepository)
        {
            var defaultAchievements = staticWebAssets.ReadJsonAsync<Dictionary<Achievement, int[]>>("json/achievement/achievements.json").Result;
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
