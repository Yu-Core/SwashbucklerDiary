using SqlSugar;
using SwashbucklerDiary.Rcl;
using SwashbucklerDiary.Shared;
using System.Text;
using System.Text.Json;

namespace SwashbucklerDiary.Rcl.Services
{
    public class WebDavDiarySyncService : IDiarySyncService
    {
        private const string EntityDiary = "Diary";
        private const string OperationUpsert = "upsert";
        private const string OperationDelete = "delete";
        private const string SyncRoot = "SwashbucklerDiary/sync/v1";
        private const string ChangesFolder = SyncRoot + "/changes";
        private const int SyncBatchVersion = 2;

        private readonly IWebDAV _webDAV;
        private readonly IDiaryService _diaryService;
        private readonly ISettingService _settingService;
        private readonly ISqlSugarClient _sqlSugarClient;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            WriteIndented = true
        };

        public WebDavDiarySyncService(IWebDAV webDAV,
            IDiaryService diaryService,
            ISettingService settingService,
            ISqlSugarClient sqlSugarClient)
        {
            _webDAV = webDAV;
            _diaryService = diaryService;
            _settingService = settingService;
            _sqlSugarClient = sqlSugarClient;
        }

        public async Task<DiarySyncResult> SyncAsync()
        {
            if (_settingService.GetTemp(it => it.PrivacyMode))
            {
                throw new InvalidOperationException("Diary sync is not available in privacy mode.");
            }

            var result = new DiarySyncResult();
            string deviceId = await GetDeviceIdAsync().ConfigureAwait(false);

            EnsureSyncTables();

            await _webDAV.EnsureFolderAsync(SyncRoot).ConfigureAwait(false);
            await _webDAV.EnsureFolderAsync(ChangesFolder).ConfigureAwait(false);
            await _webDAV.EnsureFolderAsync(GetDeviceChangesFolder(deviceId)).ConfigureAwait(false);

            result.Pushed = await PushLocalChangesAsync(deviceId).ConfigureAwait(false);
            var pullResult = await PullRemoteChangesAsync(deviceId).ConfigureAwait(false);
            result.Pulled = pullResult.Pulled;
            result.Deleted = pullResult.Deleted;
            result.Skipped = pullResult.Skipped;
            result.Conflicts = pullResult.Conflicts;
            return result;
        }

        public async Task<bool> HasLocalChangesAsync()
        {
            EnsureSyncTables();
            DateTime lastPushAt = _settingService.Get(s => s.WebDAVDiarySyncLastPushTime, DateTime.MinValue);
            bool hasDiaries = await Db.Queryable<DiaryModel>()
                .AnyAsync(it => it.UpdateTime > lastPushAt)
                .ConfigureAwait(false);
            if (hasDiaries)
            {
                return true;
            }

            return await Db.Queryable<SyncTombstoneModel>()
                .AnyAsync(it => it.EntityName == EntityDiary && it.UpdateTime > lastPushAt)
                .ConfigureAwait(false);
        }

        private async Task<int> PushLocalChangesAsync(string deviceId)
        {
            DateTime lastPushAt = _settingService.Get(s => s.WebDAVDiarySyncLastPushTime, DateTime.MinValue);
            DateTime pushedAt = DateTime.Now;
            var diaries = await _diaryService.QueryAsync(it => it.UpdateTime > lastPushAt).ConfigureAwait(false);
            var tombstones = await Db.Queryable<SyncTombstoneModel>()
                .Where(it => it.EntityName == EntityDiary && it.UpdateTime > lastPushAt)
                .ToListAsync()
                .ConfigureAwait(false);

            if (diaries.Count == 0 && tombstones.Count == 0)
            {
                return 0;
            }

            string batchId = Guid.NewGuid().ToString("N");
            var batch = new SyncBatch()
            {
                Version = SyncBatchVersion,
                BatchId = batchId,
                DeviceId = deviceId,
                CreatedAt = pushedAt,
                Diaries =
                [
                    .. diaries.Select(it => new SyncDiaryChange()
                    {
                        Operation = OperationUpsert,
                        Id = it.Id,
                        UpdatedAt = it.UpdateTime,
                        Diary = CloneDiaryWithoutRelations(it),
                        Tags = [.. (it.Tags ?? []).Select(CloneTag)],
                        Resources = [.. (it.Resources ?? []).Select(CloneResource)]
                    }),
                    .. tombstones
                    .Select(it => Guid.TryParse(it.EntityId, out Guid entityId)
                        ? new SyncDiaryChange()
                        {
                            Operation = OperationDelete,
                            Id = entityId,
                            UpdatedAt = it.DeletedAt
                        }
                        : null)
                    .Where(it => it is not null)
                    .Select(it => it!)
                ]
            };

            if (batch.Diaries.Count == 0)
            {
                await _settingService.SetAsync(s => s.WebDAVDiarySyncLastPushTime, pushedAt).ConfigureAwait(false);
                return 0;
            }

            string json = JsonSerializer.Serialize(batch, _jsonSerializerOptions);
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            string destFileName = $"{GetDeviceChangesFolder(deviceId)}/{pushedAt:yyyyMMddHHmmss}-{batchId}.json";
            await _webDAV.UploadAsync(destFileName, stream).ConfigureAwait(false);
            await _settingService.SetAsync(s => s.WebDAVDiarySyncLastPushTime, pushedAt).ConfigureAwait(false);
            return batch.Diaries.Count;
        }

        private async Task<DiarySyncResult> PullRemoteChangesAsync(string localDeviceId)
        {
            var result = new DiarySyncResult();
            var deviceFolders = await _webDAV.GetFileListAsync(ChangesFolder).ConfigureAwait(false);
            foreach (var deviceFolder in deviceFolders.Where(it => it.IsCollection))
            {
                if (deviceFolder.Name == localDeviceId)
                {
                    continue;
                }

                string folderName = $"{ChangesFolder}/{deviceFolder.Name}";
                var batchFiles = await _webDAV.GetFileListAsync(folderName, ".json").ConfigureAwait(false);
                foreach (var batchFile in batchFiles.Where(it => !it.IsCollection).OrderBy(it => it.Name))
                {
                    string path = $"{folderName}/{batchFile.Name}";
                    var batchResult = await ApplyBatchAsync(path).ConfigureAwait(false);
                    result.Pulled += batchResult.Pulled;
                    result.Deleted += batchResult.Deleted;
                    result.Skipped += batchResult.Skipped;
                    result.Conflicts += batchResult.Conflicts;
                }
            }

            return result;
        }

        private async Task<DiarySyncResult> ApplyBatchAsync(string path)
        {
            var result = new DiarySyncResult();
            bool remotePathApplied = await Db.Queryable<SyncStateModel>()
                .AnyAsync(it => it.RemotePath == path)
                .ConfigureAwait(false);
            if (remotePathApplied)
            {
                result.Skipped++;
                return result;
            }

            await using var stream = await _webDAV.DownloadAsync(path).ConfigureAwait(false);
            var batch = await JsonSerializer.DeserializeAsync<SyncBatch>(stream, _jsonSerializerOptions).ConfigureAwait(false);
            if (batch is null || string.IsNullOrWhiteSpace(batch.BatchId))
            {
                result.Skipped++;
                return result;
            }

            bool applied = await Db.Queryable<SyncStateModel>().AnyAsync(it => it.BatchId == batch.BatchId || it.RemotePath == path).ConfigureAwait(false);
            if (applied)
            {
                result.Skipped++;
                return result;
            }

            if (batch.Version > SyncBatchVersion)
            {
                result.Skipped++;
                return result;
            }

            foreach (var change in batch.Diaries)
            {
                if (change.Operation == OperationUpsert && change.Diary is not null)
                {
                    var local = await _diaryService.FindAsync(change.Id).ConfigureAwait(false);
                    if (local is null)
                    {
                        change.Diary.Tags = [.. change.Tags.Select(ToTagModel)];
                        change.Diary.Resources = [.. change.Resources.Select(ToResourceModel)];
                        await _diaryService.AddAsync(change.Diary).ConfigureAwait(false);
                        result.Pulled++;
                    }
                    else if (change.UpdatedAt > local.UpdateTime)
                    {
                        change.Diary.Tags = [.. change.Tags.Select(ToTagModel)];
                        change.Diary.Resources = [.. change.Resources.Select(ToResourceModel)];
                        await _diaryService.UpdateIncludesAsync(change.Diary).ConfigureAwait(false);
                        result.Pulled++;
                    }
                    else
                    {
                        result.Skipped++;
                        if (change.UpdatedAt < local.UpdateTime)
                        {
                            await AddConflictAsync(batch, path, change, local.UpdateTime, "Local diary is newer than remote diary.").ConfigureAwait(false);
                            result.Conflicts++;
                        }
                    }
                }
                else if (change.Operation == OperationDelete)
                {
                    var local = await _diaryService.FindAsync(change.Id).ConfigureAwait(false);
                    if (local is not null && change.UpdatedAt >= local.UpdateTime)
                    {
                        await _diaryService.DeleteFromSyncAsync(local).ConfigureAwait(false);
                        result.Deleted++;
                    }
                    else
                    {
                        result.Skipped++;
                        if (local is not null && change.UpdatedAt < local.UpdateTime)
                        {
                            await AddConflictAsync(batch, path, change, local.UpdateTime, "Local diary is newer than remote delete.").ConfigureAwait(false);
                            result.Conflicts++;
                        }
                    }
                }
            }

            await Db.Insertable(new SyncStateModel()
            {
                Id = Guid.NewGuid(),
                BatchId = batch.BatchId,
                DeviceId = batch.DeviceId,
                CreateTime = batch.CreatedAt,
                UpdateTime = DateTime.Now,
                RemotePath = path,
                AppliedAt = DateTime.Now
            }).ExecuteCommandAsync().ConfigureAwait(false);

            return result;
        }

        private Task AddConflictAsync(SyncBatch batch, string path, SyncDiaryChange change, DateTime localUpdateTime, string reason)
        {
            DateTime now = DateTime.Now;
            return Db.Storageable(new SyncConflictModel()
            {
                Id = Guid.NewGuid(),
                EntityName = EntityDiary,
                EntityId = change.Id.ToString(),
                BatchId = batch.BatchId,
                DeviceId = batch.DeviceId,
                RemotePath = path,
                LocalUpdateTime = localUpdateTime,
                RemoteUpdateTime = change.UpdatedAt,
                Reason = reason,
                CreateTime = now,
                UpdateTime = now
            })
            .WhereColumns(it => new { it.BatchId, it.EntityId, it.Reason })
            .ExecuteCommandAsync();
        }

        private async Task<string> GetDeviceIdAsync()
        {
            string deviceId = _settingService.Get(s => s.SyncDeviceId, string.Empty);
            if (!string.IsNullOrWhiteSpace(deviceId))
            {
                return deviceId;
            }

            deviceId = Guid.NewGuid().ToString("N");
            await _settingService.SetAsync(s => s.SyncDeviceId, deviceId).ConfigureAwait(false);
            return deviceId;
        }

        private string GetDeviceChangesFolder(string deviceId) => $"{ChangesFolder}/{deviceId}";

        private ISqlSugarClient Db => _sqlSugarClient.AsTenant().GetConnection(SQLiteConstants.MainDatabaseFilename);

        private void EnsureSyncTables()
        {
            Db.CodeFirst.InitTables<SyncStateModel, SyncTombstoneModel, SyncConflictModel>();
        }

        private static DiaryModel CloneDiaryWithoutRelations(DiaryModel diary)
            => new()
            {
                Id = diary.Id,
                CreateTime = diary.CreateTime,
                UpdateTime = diary.UpdateTime,
                Title = diary.Title,
                Content = diary.Content,
                Mood = diary.Mood,
                Weather = diary.Weather,
                Location = diary.Location,
                Top = diary.Top,
                Template = diary.Template
            };

        private static SyncTagSnapshot CloneTag(TagModel tag)
            => new()
            {
                Id = tag.Id,
                CreateTime = tag.CreateTime,
                UpdateTime = tag.UpdateTime,
                Name = tag.Name
            };

        private static SyncResourceSnapshot CloneResource(ResourceModel resource)
            => new()
            {
                ResourceUri = resource.ResourceUri,
                ResourceType = resource.ResourceType
            };

        private static TagModel ToTagModel(SyncTagSnapshot tag)
            => new()
            {
                Id = tag.Id,
                CreateTime = tag.CreateTime,
                UpdateTime = tag.UpdateTime,
                Name = tag.Name
            };

        private static ResourceModel ToResourceModel(SyncResourceSnapshot resource)
            => new()
            {
                ResourceUri = resource.ResourceUri,
                ResourceType = resource.ResourceType
            };
    }
}
