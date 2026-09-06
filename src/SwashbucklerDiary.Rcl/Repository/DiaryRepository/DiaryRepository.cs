using SqlSugar;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Repository
{
    public class DiaryRepository : BaseRepository<DiaryModel>, IDiaryRepository
    {
        public DiaryRepository(ISqlSugarClient context,
            ISettingService settingService) : base(context, settingService)
        {
        }

        public override Task<bool> InsertAsync(DiaryModel model)
        {
            return Context.InsertNav(model)
            .Include(it => it.Tags)
            .Include(it => it.Resources)
            .ExecuteCommandAsync();
        }

        public override Task<bool> DeleteAsync(DiaryModel model)
        {
            return Context.DeleteNav(model)
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
            return await Context.Queryable<DiaryTagModel>()
                .Where(dt => dt.DiaryId == id)
                .LeftJoin<TagModel>((dt, t) => dt.TagId == t.Id)
                .Select((dt, t) => t)
                .ToListAsync();
        }

        public override Task<List<DiaryModel>> GetListAsync()
        {
            return Context.Queryable<DiaryModel>()
                .Includes(it => it.Tags)
                .Includes(it => it.Resources)
                .OrderByDescending(it => it.CreateTime)
                .ToListAsync();
        }

        public override Task<List<DiaryModel>> GetListAsync(Expression<Func<DiaryModel, bool>> expression)
            => InternalGetListAsync(Context, expression);

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
            return Context.UpdateNav(model)
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
            return Context.UpdateNav(models)
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
            return Context.UpdateNav(model)
            .Include(it => it.Tags)
            .ExecuteCommandAsync();
        }

        public Task<bool> ImportAsync(List<DiaryModel> diaries)
            => InternalImportAsync(Context, diaries);

        public static async Task<bool> InternalImportAsync(ISqlSugarClient context, List<DiaryModel> diaries)
        {
            await MergeSameNameTagsAsync(context, diaries).ConfigureAwait(false);

            return await context.UpdateNav(diaries, new UpdateNavRootOptions()
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
            .ExecuteCommandAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// UpdateNav matches many-to-many children only by primary key, so imported diaries
        /// would insert duplicate tags when their Name matches an existing tag but their Id
        /// comes from another database (e.g. a backup from another device). Remap same-name
        /// imported tags to the existing tags (or to each other) before writing.
        /// </summary>
        private static async Task MergeSameNameTagsAsync(ISqlSugarClient context, List<DiaryModel> diaries)
        {
            var importedTags = diaries
                .Where(it => it.Tags is not null)
                .SelectMany(it => it.Tags!)
                .ToList();

            if (importedTags.Count == 0)
            {
                return;
            }

            var importedTagNames = importedTags
                 .Select(t => t.Name)
                 .Where(n => n is not null)
                 .Distinct()
                 .ToList();

            var tags = await context.Queryable<TagModel>()
                .Where(t => t.Name != null && importedTagNames.Contains(t.Name))
                .OrderBy(t => t.CreateTime)
                .ToListAsync();

            var tagByName = new Dictionary<string, TagModel>();
            foreach (var tag in tags)
            {
                tagByName.TryAdd(tag.Name!, tag);
            }

            foreach (var tag in importedTags)
            {
                if (tag.Name is null)
                {
                    continue;
                }

                if (tagByName.TryGetValue(tag.Name, out var sameNameTag))
                {
                    // Link the imported diary to the existing tag instead of creating a duplicate
                    tag.Id = sameNameTag.Id;
                    tag.Name = sameNameTag.Name;
                    tag.CreateTime = sameNameTag.CreateTime;
                    tag.UpdateTime = sameNameTag.UpdateTime;
                }
                else
                {
                    // Unify tags of the same name within the imported data itself
                    tagByName.Add(tag.Name, tag);
                }
            }

            foreach (var diary in diaries)
            {
                diary.Tags = diary.Tags?.DistinctBy(it => it.Id).ToList();
            }
        }

        public async Task<bool> MovePrivacyDiaryAsync(DiaryModel diary, bool toPrivacyMode)
        {
            var db = Itenant.GetConnection(SQLiteConstants.MainDatabaseFilename);
            var privacyDb = Itenant.GetConnection(SQLiteConstants.PrivacyDatabaseFilename);
            var (from, to) = toPrivacyMode ? (db, privacyDb) : (privacyDb, db);
            bool flag = await InternalImportAsync(to, [diary]).ConfigureAwait(false);
            if (!flag)
            {
                return false;
            }

            bool flag2 = await InternalDeleteAsync(from, [diary]).ConfigureAwait(false);
            if (!flag2)
            {
                return false;
            }
            return true;
        }

#pragma warning disable CS0618 // 类型或成员已过时
        public async Task<bool> MovePrivacyDiariesAsync()
        {
            var db = Itenant.GetConnection(SQLiteConstants.MainDatabaseFilename);
            var privacyDb = Itenant.GetConnection(SQLiteConstants.PrivacyDatabaseFilename);

            var diaries = await InternalGetListAsync(db, it => it.Private == true).ConfigureAwait(false);

            if (diaries.Count == 0)
            {
                return false;
            }

            diaries.ForEach(it => it.Private = false);
            bool flag = await InternalImportAsync(privacyDb, diaries).ConfigureAwait(false);
            if (!flag)
            {
                return false;
            }

            bool flag2 = await InternalDeleteAsync(db, diaries).ConfigureAwait(false);
            if (!flag2)
            {
                return false;
            }

            return true;
        }
#pragma warning restore CS0618 // 类型或成员已过时
    }
}
