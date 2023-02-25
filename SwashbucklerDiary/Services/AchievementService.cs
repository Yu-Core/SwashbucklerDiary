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
            new(1,AchievementType.Diary,1),
            new(2,AchievementType.Diary,30),
            new(3, AchievementType.Diary, 100),
            new(4, AchievementType.Diary, 1000),
            new(5, AchievementType.Word, 3000),
            new(6, AchievementType.Word, 10000),
            new(7, AchievementType.Word, 100000),
            new(8, AchievementType.SourceCode, 1),
            new(9, AchievementType.Avatar, 1),
            new(10, AchievementType.NickName, 1),
            new(11, AchievementType.Sign, 1),
            new(12, AchievementType.Log, 1),
            new(13, AchievementType.Share, 1),
            new(14, AchievementType.Export, 1),
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
            var userState = await _userStateModelRepository.InsertOrUpdateAsync(type,count);
            return await CheckAchievement(userState!);
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
