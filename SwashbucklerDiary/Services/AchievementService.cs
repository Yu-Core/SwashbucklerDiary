using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Services
{
    public class AchievementService : IAchievementService
    {
        private readonly IUserAchievementRepository _userAchievementRepository;
        private readonly IUserStateModelRepository _userStateModelRepository;
        private readonly List<AchievementModel> Achievements = new()
        {
            new(AchievementType.Diary,1),
            new(AchievementType.Diary,30),
            new(AchievementType.Diary, 100),
            new(AchievementType.Diary, 1000),
            new(AchievementType.Word, 3000),
            new(AchievementType.Word, 10000),
            new(AchievementType.Word, 100000),
            new(AchievementType.SourceCode, 1),
            new(AchievementType.Avatar, 1),
            new(AchievementType.NickName, 1),
            new(AchievementType.Sign, 1),
            new(AchievementType.Log, 1),
            new(AchievementType.Share, 1),
            new(AchievementType.Export, 1),
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

        public async Task<List<string>> UpdateUserState(AchievementType type, int count)
        {
            var userState = await _userStateModelRepository.InsertOrUpdateAsync(type, count);
            return await CheckAchievement(userState!);
        }

        private async Task<List<string>> CheckAchievement(UserStateModel userState)
        {
            var type = userState.Type;
            var achievements = Achievements.Where(it => it.Type == type).ToList();
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
