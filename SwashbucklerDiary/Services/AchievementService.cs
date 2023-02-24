using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models.Data;

namespace SwashbucklerDiary.Services
{
    public class AchievementService : IAchievementService
    {
        private readonly IUserAchievementRepository _userAchievementRepository;
        private readonly IUserStateModelRepository _userStateModelRepository;
        private readonly List<AchievementModel> Achievements = new()
        {
            new(1,"Memory start",AchievementType.Diary,"Keep a diary for the first time",1),
            new(2,"Unremitting",AchievementType.Diary,"Keep a diary for 30 days in total",30),
            new(3,"Many a little make a mickle",AchievementType.Diary,"Keep 30 diaries in total",30),
        };

        public AchievementService(IUserAchievementRepository userAchievementRepository, 
            IUserStateModelRepository userStateModelRepository)
        {
            _userAchievementRepository = userAchievementRepository;
            _userStateModelRepository = userStateModelRepository;
        }

        public async Task<List<string>> UpdateUserState(AchievementType type)
        {
            var userState = await _userStateModelRepository.InsertOrUpdateAsync(type);
            return await CheckAchievement(userState!);
        }

        public async Task<List<string>> UpdateUserState(UserStateModel userState)
        {
            var newUserState = await _userStateModelRepository.InsertOrUpdateAsync(userState);
            return await CheckAchievement(newUserState!);
        }

        private async Task<List<string>> CheckAchievement(UserStateModel userState)
        {
            var type = userState.Type;
            var achievements = Achievements.Where(it => it.Type == type).ToList();
            List<string> messages = new();
            foreach (var item in achievements)
            {
                var userAchievement = await _userAchievementRepository.GetFirstAsync(it => it.AchievementId == item.Id);
                if (userAchievement == null)
                {
                    userAchievement = new UserAchievementModel()
                    {
                        AchievementId = item.Id
                    };
                    userAchievement = await _userAchievementRepository.InsertReturnEntityAsync(userAchievement);
                }

                if (userAchievement.IsCompleted)
                {
                    continue;
                }

                if(userAchievement.CompleteRate == userState.Count)
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
    }
}
