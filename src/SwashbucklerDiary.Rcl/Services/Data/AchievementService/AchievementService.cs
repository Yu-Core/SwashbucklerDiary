using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public class AchievementService : IAchievementService
    {
        protected readonly IUserAchievementRepository _userAchievementRepository;

        protected readonly IUserStateModelRepository _userStateModelRepository;

        protected readonly IGlobalConfiguration _globalConfiguration;

        protected List<AchievementModel> Achievements => _globalConfiguration.Achievements;

        public event Action<UserStateModel>? UserStateChanged;

        public AchievementService(IUserAchievementRepository userAchievementRepository,
            IUserStateModelRepository userStateModelRepository,
            IGlobalConfiguration globalConfiguration)
        {
            _userAchievementRepository = userAchievementRepository;
            _userStateModelRepository = userStateModelRepository;
            _globalConfiguration = globalConfiguration;
        }

        public async Task<List<string>> UpdateUserState(Achievement type)
        {
            var userState = await _userStateModelRepository.InsertOrUpdateAsync(type);
            UserStateChanged?.Invoke(userState);
            return await CheckAchievement(userState!);
        }

        public async Task<List<string>> UpdateUserState(Achievement type, int count)
        {
            var userState = await _userStateModelRepository.InsertOrUpdateAsync(type, count);
            UserStateChanged?.Invoke(userState);
            return await CheckAchievement(userState!);
        }

        private async Task<List<string>> CheckAchievement(UserStateModel userState)
        {
            var type = userState.Type;
            var achievements = Achievements.Where(it => it.Kind == type).ToList();
            List<string> messages = [];
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
