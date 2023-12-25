using SwashbucklerDiary.Maui.IRepository;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Services
{
    public class AchievementService : IAchievementService
    {
        private readonly IUserAchievementRepository _userAchievementRepository;

        private readonly IUserStateModelRepository _userStateModelRepository;

        private readonly List<AchievementModel> Achievements = new();

        public AchievementService(IUserAchievementRepository userAchievementRepository,
            IUserStateModelRepository userStateModelRepository,
            IStaticWebAssets staticWebAssets)
        {
            _userAchievementRepository = userAchievementRepository;
            _userStateModelRepository = userStateModelRepository;
            var defaultAchievements = staticWebAssets.ReadJsonAsync<Dictionary<Achievement, int[]>>("json/achievement/achievements.json").Result;
            foreach (var item in defaultAchievements)
            {
                foreach (var count in item.Value)
                {
                    Achievements.Add(new(item.Key, count));
                }
            }
        }

        public async Task<List<string>> UpdateUserState(Achievement type)
        {
            var userState = await _userStateModelRepository.InsertOrUpdateAsync(type);
            return await CheckAchievement(userState!);
        }

        public async Task<List<string>> UpdateUserState(Achievement type, int count)
        {
            var userState = await _userStateModelRepository.InsertOrUpdateAsync(type, count);
            return await CheckAchievement(userState!);
        }

        private async Task<List<string>> CheckAchievement(UserStateModel userState)
        {
            var type = userState.Type;
            var achievements = Achievements.Where(it => it.Kind == type).ToList();
            List<string> messages = new();
            foreach (var item in achievements)
            {
                var userAchievement = await _userAchievementRepository.GetFirstAsync(it => it.AchievementName == item.Name);
                if (userAchievement == null)
                {
                    userAchievement = new UserAchievementModel()
                    {
                        AchievementName = item.Name
                    };
                    userAchievement = await _userAchievementRepository.InsertReturnEntityAsync(userAchievement);
                }

                if (userAchievement.IsCompleted)
                {
                    continue;
                }

                if (userAchievement.CompleteRate == userState.Count)
                {
                    continue;
                }

                userAchievement.CompleteRate = Math.Min(userState.Count, item.Steps);
                if (item.Steps == userAchievement.CompleteRate)
                {
                    messages.Add(item.Name);
                    userAchievement.IsCompleted = true;
                    userAchievement.CompletedTime = DateTime.Now;
                }

                await _userAchievementRepository.UpdateAsync(userAchievement);
            }
            return messages;
        }

        public async Task<List<AchievementModel>> GetAchievements()
        {
            var userAchievements = await _userAchievementRepository.GetListAsync();
            foreach (var item in Achievements)
            {
                item.UserAchievement = userAchievements.FirstOrDefault(it => it.AchievementName == item.Name) ?? new();
            }

            return Achievements;
        }
    }
}
