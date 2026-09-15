using SqlSugar;
using SwashbucklerDiary.Rcl;
using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Services
{
#pragma warning disable CS0472 // 由于此类型的值永不等于 "null"，该表达式的结果始终相同
    public class DiaryService : BaseDataService<DiaryModel>, IDiaryService
    {
        private readonly IDiaryRepository _iDiaryRepository;
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ISettingService _settingService;

        public DiaryService(IDiaryRepository iDiaryRepository,
            ISqlSugarClient sqlSugarClient,
            ISettingService settingService)
        {
            base._iBaseRepository = iDiaryRepository;
            _iDiaryRepository = iDiaryRepository;
            _sqlSugarClient = sqlSugarClient;
            _settingService = settingService;
        }

        public override Task<List<DiaryModel>> QueryAsync()
        {
            return _iBaseRepository.GetListAsync();
        }

        public override Task<List<DiaryModel>> QueryAsync(Expression<Func<DiaryModel, bool>> expression)
        {
            return _iBaseRepository.GetListAsync(expression);
        }

        public Task<List<TagModel>> GetTagsAsync(Guid id)
        {
            return _iDiaryRepository.GetTagsAsync(id);
        }

        public override async Task<bool> DeleteAsync(DiaryModel entity)
        {
            bool flag = await base.DeleteAsync(entity).ConfigureAwait(false);
            if (flag)
            {
                await AddTombstoneAsync(entity.Id).ConfigureAwait(false);
            }

            return flag;
        }

        public override async Task<bool> DeleteAsync(Guid id)
        {
            bool flag = await base.DeleteAsync(id).ConfigureAwait(false);
            if (flag)
            {
                await AddTombstoneAsync(id).ConfigureAwait(false);
            }

            return flag;
        }

        public override async Task<bool> DeleteAsync(List<DiaryModel> entities)
        {
            bool flag = await base.DeleteAsync(entities).ConfigureAwait(false);
            if (flag)
            {
                await AddTombstonesAsync(entities.Select(it => it.Id)).ConfigureAwait(false);
            }

            return flag;
        }

        public Task<bool> UpdateIncludesAsync(DiaryModel model)
        {
            return _iDiaryRepository.UpdateIncludesAsync(model);
        }

        public Task<bool> UpdateIncludesAsync(List<DiaryModel> models)
        {
            return _iDiaryRepository.UpdateIncludesAsync(models);
        }

        public Task<bool> UpdateTagsAsync(DiaryModel model)
        {
            return _iDiaryRepository.UpdateTagsAsync(model);
        }

        public Task<bool> ImportAsync(List<DiaryModel> diaries)
        {
            return _iDiaryRepository.ImportAsync(diaries);
        }

        public Task<bool> MovePrivacyDiaryAsync(DiaryModel diary, bool toPrivacyMode)
        {
            return _iDiaryRepository.MovePrivacyDiaryAsync(diary, toPrivacyMode);
        }

        public Task<bool> MovePrivacyDiariesAsync()
        {
            return _iDiaryRepository.MovePrivacyDiariesAsync();
        }

        public Task<bool> DeleteFromSyncAsync(DiaryModel diary)
        {
            return base.DeleteAsync(diary);
        }

        public Task<List<DiaryModel>> QueryDiariesAsync()
        {
            return QueryAsync(it => it.Template == null || it.Template == false);
        }

        public Task<List<DiaryModel>> QueryDiariesAsync(Expression<Func<DiaryModel, bool>> expression)
        {
            var expable = Expressionable.Create<DiaryModel>();
            expable.And(expression);
            expable.And(it => it.Template == null || it.Template == false);
            return QueryAsync(expable.ToExpression());
        }

        public Task<List<DiaryModel>> QueryTemplatesAsync()
        {
            return QueryAsync(it => it.Template);
        }

        public Task<List<DiaryModel>> QueryTemplatesAsync(Expression<Func<DiaryModel, bool>> expression)
        {
            var expable = Expressionable.Create<DiaryModel>();
            expable.And(expression);
            expable.And(it => it.Template);
            return QueryAsync(expable.ToExpression());
        }

        private Task AddTombstoneAsync(Guid id)
            => AddTombstonesAsync([id]);

        private Task AddTombstonesAsync(IEnumerable<Guid> ids)
        {
            if (_settingService.GetTemp(it => it.PrivacyMode))
            {
                return Task.CompletedTask;
            }

            string deviceId = _settingService.Get(s => s.SyncDeviceId, string.Empty);
            DateTime now = DateTime.Now;
            var tombstones = ids.Select(id => new SyncTombstoneModel()
            {
                Id = Guid.NewGuid(),
                EntityName = "Diary",
                EntityId = id.ToString(),
                DeviceId = deviceId,
                DeletedAt = now,
                CreateTime = now,
                UpdateTime = now
            }).ToList();

            if (tombstones.Count == 0)
            {
                return Task.CompletedTask;
            }

            return _sqlSugarClient.AsTenant()
                .GetConnection(SQLiteConstants.MainDatabaseFilename)
                .Insertable(tombstones)
                .ExecuteCommandAsync();
        }
    }
}
