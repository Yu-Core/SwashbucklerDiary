using SqlSugar;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Repository
{
    public class UserStateModelRepository : BaseRepository<UserStateModel>, IUserStateModelRepository
    {
        public UserStateModelRepository(ISqlSugarClient context) : base(context)
        {
        }

        public async Task<UserStateModel> InsertOrUpdateAsync(Achievement type)
        {
            var userState = await base.GetFirstAsync(it => it.Type == type).ConfigureAwait(false);
            if (userState == null)
            {
                UserStateModel newUserState = new()
                {
                    Type = type,
                    Count = 1,
                };
                await base.InsertAsync(newUserState).ConfigureAwait(false);
                return newUserState;
            }
            else
            {
                await Context.Updateable<UserStateModel>()
                .SetColumns(it => it.Count == it.Count + 1)
                .Where(it => it.Type == type)
                .ExecuteCommandAsync()
                .ConfigureAwait(false);
                return new()
                {
                    Type = type,
                    Count = userState.Count + 1
                };
            }

        }

        public async Task<UserStateModel> InsertOrUpdateAsync(Achievement type, int count)
        {
            UserStateModel userState = new()
            {
                Type = type,
                Count = count
            };
            var oldUserState = await base.GetFirstAsync(it => it.Type == type).ConfigureAwait(false);
            if (oldUserState == null)
            {
                await base.InsertAsync(userState).ConfigureAwait(false);
            }
            else
            {
                await Context.Updateable(userState)
                    .UpdateColumns(it => new { it.Count })
                    .Where(it => it.Type == type)
                    .ExecuteCommandAsync()
                    .ConfigureAwait(false);
            }

            return userState;
        }
    }
}
