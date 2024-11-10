using SqlSugar;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Repository
{
    public class DiaryRepository : BaseRepository<DiaryModel>, IDiaryRepository
    {
        public DiaryRepository(ISqlSugarClient context) : base(context)
        {
        }

        public override Task<bool> InsertAsync(DiaryModel model)
        {
            return base.Context.InsertNav(model)
            .Include(it => it.Tags)
            .Include(it => it.Resources)
            .ExecuteCommandAsync();
        }

        public override Task<bool> DeleteAsync(DiaryModel model)
        {
            return base.Context.DeleteNav(model)
                .Include(it => it.Tags, new DeleteNavOptions()
                {
                    ManyToManyIsDeleteA = true
                })
                .Include(it => it.Resources, new DeleteNavOptions()
                {
                    ManyToManyIsDeleteA = true
                })
                .ExecuteCommandAsync();
        }

        private static Task<bool> InternalDeleteAsync(ISqlSugarClient context, List<DiaryModel> models)
        {
            return context.DeleteNav(models)
                .Include(it => it.Tags, new DeleteNavOptions()
                {
                    ManyToManyIsDeleteA = true
                })
                .Include(it => it.Resources, new DeleteNavOptions()
                {
                    ManyToManyIsDeleteA = true
                })
                .ExecuteCommandAsync();
        }

        public override Task<DiaryModel> GetByIdAsync(dynamic id)
        {
            return Context.Queryable<DiaryModel>()
                .Includes(it => it.Tags)
                .Includes(it => it.Resources)
                .InSingleAsync(id);
        }

        public override Task<DiaryModel> GetFirstAsync(Expression<Func<DiaryModel, bool>> expression)
        {
            return Context.Queryable<DiaryModel>()
                .Includes(it => it.Tags)
                .Includes(it => it.Resources)
                .FirstAsync(expression);
        }

        public async Task<List<TagModel>> GetTagsAsync(Guid id)
        {
            return await base.Context.Queryable<DiaryTagModel>()
                .Where(dt => dt.DiaryId == id)
                .LeftJoin<TagModel>((dt, t) => dt.TagId == t.Id)
                .Select((dt, t) => t)
                .ToListAsync();
        }

        public override Task<List<DiaryModel>> GetListAsync()
        {
            return base.Context.Queryable<DiaryModel>()
                .Includes(it => it.Tags)
                .Includes(it => it.Resources)
                .OrderByDescending(it => it.CreateTime)
                .ToListAsync();
        }

        public override Task<List<DiaryModel>> GetListAsync(Expression<Func<DiaryModel, bool>> expression)
            => InternalGetListAsync(base.Context, expression);

        private static Task<List<DiaryModel>> InternalGetListAsync(ISqlSugarClient context, Expression<Func<DiaryModel, bool>> expression)
        {
            return context.Queryable<DiaryModel>()
                .Includes(it => it.Tags)
                .Includes(it => it.Resources)
                .Where(expression)
                .OrderByDescending(it => it.CreateTime)
                .ToListAsync();
        }

        public Task<bool> UpdateIncludesAsync(DiaryModel model)
        {
            return base.Context.UpdateNav(model)
            .Include(it => it.Tags, new UpdateNavOptions
            {
                ManyToManyIsUpdateA = true
            })
            .Include(it => it.Resources, new UpdateNavOptions
            {
                ManyToManyIsUpdateA = true,
                ManyToManyIsUpdateB = true,
            })
            .ExecuteCommandAsync();
        }

        public Task<bool> UpdateIncludesAsync(List<DiaryModel> models)
        {
            return base.Context.UpdateNav(models)
            .Include(it => it.Tags, new UpdateNavOptions
            {
                ManyToManyIsUpdateA = true
            })
            .Include(it => it.Resources, new UpdateNavOptions
            {
                ManyToManyIsUpdateA = true,
                ManyToManyIsUpdateB = true,
            })
            .ExecuteCommandAsync();
        }

        public Task<bool> UpdateTagsAsync(DiaryModel model)
        {
            return base.Context.UpdateNav(model)
            .Include(it => it.Tags)
            .ExecuteCommandAsync();
        }

        public Task<bool> ImportAsync(List<DiaryModel> diaries)
            => InternalImportAsync(base.Context, diaries);

        public static Task<bool> InternalImportAsync(ISqlSugarClient context, List<DiaryModel> diaries)
        {
            return context.UpdateNav(diaries, new UpdateNavRootOptions()
            {
                IsInsertRoot = true
            })
            .Include(it => it.Tags, new UpdateNavOptions
            {
                ManyToManyIsUpdateA = true,
                ManyToManyIsUpdateB = true
            })
            .Include(it => it.Resources, new UpdateNavOptions
            {
                ManyToManyIsUpdateA = true,
                ManyToManyIsUpdateB = true
            })
            .ExecuteCommandAsync();
        }

        public async Task<bool> MovePrivacyDiaryAsync(DiaryModel diary, bool toPrivacyMode)
        {
            var db = Context.AsTenant().GetConnection("0");
            var privacyDb = Context.AsTenant().GetConnection("1");
            var (from, to) = toPrivacyMode ? (db, privacyDb) : (privacyDb, db);
            bool flag = await InternalImportAsync(to, [diary]);
            if (!flag)
            {
                return false;
            }

            bool flag2 = await InternalDeleteAsync(from, [diary]);
            if (!flag2)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> MovePrivacyDiariesAsync()
        {
            var db = Context.AsTenant().GetConnection("0");
            var privacyDb = Context.AsTenant().GetConnection("1");
            var diaries = await InternalGetListAsync(db, it => it.Private == true);
            if (diaries.Count == 0)
            {
                return false;
            }

            diaries.ForEach(it => it.Private = false);
            bool flag = await InternalImportAsync(privacyDb, diaries);
            if (!flag)
            {
                return false;
            }

            bool flag2 = await InternalDeleteAsync(db, diaries);
            if (!flag2)
            {
                return false;
            }

            return true;
        }
    }
}
