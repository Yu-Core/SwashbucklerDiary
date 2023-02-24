using SqlSugar;
using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.Models.Data;
using static Masa.Blazor.Presets.Message;

namespace SwashbucklerDiary.Repository
{
    public class UserStateModelRepository : BaseRepository<UserStateModel>, IUserStateModelRepository
    {
        public UserStateModelRepository(ISqlSugarClient context) : base(context)
        {
        }

        public async Task<UserStateModel> InsertOrUpdateAsync(AchievementType type)
        {
            UserStateModel newUserState = new()
            {
                Type = type,
                Count = 1
            };
            var userState = await base.GetFirstAsync(it => it.Type == type);
            if(userState == null)
            {
                await base.InsertAsync(newUserState);
                return newUserState;
            }
            else
            {
                await base.Context.Updateable<UserStateModel>()
                .SetColumns(it => it.Count == it.Count + 1)
                .Where(it => it.Type == type)
                .ExecuteCommandAsync();
                return new()
                {
                    Type = type,
                    Count = userState.Count + 1
                };
            }

        }

        public new async Task<UserStateModel> InsertOrUpdateAsync(UserStateModel userState)
        {
            var type = userState.Type;
            var oldUserState = await base.GetFirstAsync(it => it.Type == type);
            if (oldUserState == null)
            {
                await base.InsertAsync(userState);
            }
            else
            {
                await base.Context.Updateable(userState)
                .Where(it => it.Type == type)
                .ExecuteCommandAsync();
            }

            return userState;
        }
    }
}
