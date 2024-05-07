using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public abstract class AchievementService : IAchievementService
    {
        protected readonly IUserAchievementRepository _userAchievementRepository;

        protected readonly IUserStateModelRepository _userStateModelRepository;

        protected readonly List<AchievementModel> _achievements = [];

        public AchievementService(IUserAchievementRepository userAchievementRepository,
            IUserStateModelRepository userStateModelRepository)
        {
            _userAchievementRepository = userAchievementRepository;
            _userStateModelRepository = userStateModelRepository;
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
            var achievements = _achievements.Where(it => it.Kind == type).ToList();
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
            foreach (var item in _achievements)
            {
                item.UserAchievement = userAchievements.FirstOrDefault(it => it.AchievementName == item.Name) ?? new();
            }

            return _achievements;
        }
    }
}
