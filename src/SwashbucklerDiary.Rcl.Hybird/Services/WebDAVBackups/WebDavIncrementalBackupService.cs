using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace SwashbucklerDiary.Rcl.Services
{
    public class WebDavIncrementalBackupService : IWebDavIncrementalBackupService
    {
        private const string BackupRoot = "SwashbucklerDiary/backup-v2";
        private const string FilesRoot = BackupRoot + "/files";
        private const string ManifestsRoot = BackupRoot + "/manifests";

        private readonly IWebDAV _webDAV;
        private readonly IDiaryFileManager _diaryFileManager;
        private readonly IResourceService _resourceService;
        private readonly IMediaResourceManager _mediaResourceManager;
        private readonly IPlatformIntegration _platformIntegration;
        private readonly IAppFileSystem _appFileSystem;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            WriteIndented = true
        };

        public WebDavIncrementalBackupService(IWebDAV webDAV,
            IDiaryFileManager diaryFileManager,
            IResourceService resourceService,
            IMediaResourceManager mediaResourceManager,
            IPlatformIntegration platformIntegration,
            IAppFileSystem appFileSystem)
        {
            _webDAV = webDAV;
            _diaryFileManager = diaryFileManager;
            _resourceService = resourceService;
            _mediaResourceManager = mediaResourceManager;
            _platformIntegration = platformIntegration;
            _appFileSystem = appFileSystem;
        }

        public async Task<WebDavIncrementalBackupManifest> UploadAsync(bool includeDiaryResources)
        {
            await EnsureFoldersAsync().ConfigureAwait(false);

            string backupId = DateTime.Now.ToString("yyyyMMddHHmmss");
            var manifest = new WebDavIncrementalBackupManifest()
            {
                BackupId = backupId,
                CreatedAt = DateTime.Now,
                AppVersion = _platformIntegration.AppVersionString
            };

            string dbBackupPath = await _diaryFileManager.ExportDatabaseSnapshotAsync().ConfigureAwait(false);
            manifest.Files.Add(await UploadFileByHashAsync(dbBackupPath, "database").ConfigureAwait(false));

            if (includeDiaryResources)
            {
                var resources = await _resourceService.QueryAsync().ConfigureAwait(false);
                foreach (var resource in resources.Where(it => !string.IsNullOrWhiteSpace(it.ResourceUri)))
                {
                    string filePath = _mediaResourceManager.RelativeUrlToFilePath(resource.ResourceUri);
                    if (!File.Exists(filePath))
                    {
                        continue;
                    }

                    var file = await UploadFileByHashAsync(filePath, "media").ConfigureAwait(false);
                    file.ResourceUri = resource.ResourceUri;
                    manifest.Files.Add(file);
                }
            }

            string json = JsonSerializer.Serialize(manifest, _jsonSerializerOptions);
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _webDAV.UploadAsync($"{ManifestsRoot}/{backupId}.json", stream).ConfigureAwait(false);
            return manifest;
        }

        public async Task<List<WebDavIncrementalBackupManifest>> GetManifestsAsync()
        {
            await EnsureFoldersAsync().ConfigureAwait(false);

            var manifests = await GetIncrementalManifestsAsync().ConfigureAwait(false);
            manifests.AddRange(await GetLegacyZipBackupsAsync().ConfigureAwait(false));

            return manifests
                .OrderByDescending(it => it.CreatedAt)
                .ToList();
        }

        public async Task<WebDavBackupDiagnostics> DiagnoseAsync()
        {
            try
            {
                await EnsureFoldersAsync().ConfigureAwait(false);
                var incrementalManifests = await GetIncrementalManifestsAsync().ConfigureAwait(false);
                var legacyBackups = await GetLegacyZipBackupsAsync().ConfigureAwait(false);
                var backups = incrementalManifests.Concat(legacyBackups).ToList();

                return new WebDavBackupDiagnostics()
                {
                    Success = true,
                    IncrementalBackupCount = incrementalManifests.Count,
                    LegacyBackupCount = legacyBackups.Count,
                    LatestBackupTime = backups.Count == 0 ? null : backups.Max(it => it.CreatedAt),
                    TotalSize = backups.Sum(GetManifestSize)
                };
            }
            catch (Exception e)
            {
                return new WebDavBackupDiagnostics()
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task RestoreAsync(WebDavIncrementalBackupManifest manifest)
        {
            if (manifest.LegacyZip && !string.IsNullOrWhiteSpace(manifest.LegacyRemotePath))
            {
                await using var legacyStream = await _webDAV.DownloadAsync(manifest.LegacyRemotePath).ConfigureAwait(false);
                await _diaryFileManager.ImportDBAsync(legacyStream).ConfigureAwait(false);
                return;
            }

            var database = manifest.Files.FirstOrDefault(it => it.Kind == "database");
            if (database is null)
            {
                throw new InvalidOperationException("Backup manifest does not contain a database file.");
            }

            await using (var stream = await _webDAV.DownloadAsync(database.RemotePath).ConfigureAwait(false))
            {
                await _diaryFileManager.ImportDatabaseSnapshotAsync(stream).ConfigureAwait(false);
            }

            foreach (var file in manifest.Files.Where(it => it.Kind == "media" && !string.IsNullOrWhiteSpace(it.ResourceUri)))
            {
                string targetPath = _mediaResourceManager.RelativeUrlToFilePath(file.ResourceUri!);
                if (string.IsNullOrWhiteSpace(targetPath))
                {
                    continue;
                }

                string? directory = Path.GetDirectoryName(targetPath);
                if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (File.Exists(targetPath) && await FileHashEqualsAsync(targetPath, file.Hash).ConfigureAwait(false))
                {
                    continue;
                }

                await using var input = await _webDAV.DownloadAsync(file.RemotePath).ConfigureAwait(false);
                await using var output = File.Create(targetPath);
                await input.CopyToAsync(output).ConfigureAwait(false);
            }

            await _appFileSystem.SyncFS().ConfigureAwait(false);
        }

        private async Task<List<WebDavIncrementalBackupManifest>> GetLegacyZipBackupsAsync()
        {
            var files = await _webDAV.GetZipFileListAsync("SwashbucklerDiary").ConfigureAwait(false);
            return files
                .Where(it => !it.IsCollection)
                .Select(file => new WebDavIncrementalBackupManifest()
                {
                    BackupId = Path.GetFileNameWithoutExtension(file.Name),
                    CreatedAt = file.LastModified ?? TryGetCreatedAtFromLegacyFileName(file.Name) ?? DateTime.MinValue,
                    LegacyZip = true,
                    LegacyRemotePath = $"SwashbucklerDiary/{file.Name}",
                    LegacyLength = file.Length
                })
                .ToList();
        }

        private async Task<List<WebDavIncrementalBackupManifest>> GetIncrementalManifestsAsync()
        {
            var files = await _webDAV.GetFileListAsync(ManifestsRoot, ".json").ConfigureAwait(false);
            var manifests = new List<WebDavIncrementalBackupManifest>();
            foreach (var file in files.Where(it => !it.IsCollection).OrderByDescending(it => it.Name))
            {
                await using var stream = await _webDAV.DownloadAsync($"{ManifestsRoot}/{file.Name}").ConfigureAwait(false);
                var manifest = await JsonSerializer.DeserializeAsync<WebDavIncrementalBackupManifest>(stream, _jsonSerializerOptions).ConfigureAwait(false);
                if (manifest is not null && manifest.Files.Any(it => it.Kind == "database"))
                {
                    manifests.Add(manifest);
                }
            }

            return manifests;
        }

        private static long GetManifestSize(WebDavIncrementalBackupManifest manifest)
            => manifest.LegacyLength ?? manifest.Files.Sum(it => it.Length);

        private static DateTime? TryGetCreatedAtFromLegacyFileName(string fileName)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var parts = fileNameWithoutExtension.Split('_', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                return null;
            }

            return DateTime.TryParseExact(parts[1], "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime)
                ? dateTime
                : null;
        }

        private async Task EnsureFoldersAsync()
        {
            await _webDAV.EnsureFolderAsync(BackupRoot).ConfigureAwait(false);
            await _webDAV.EnsureFolderAsync(FilesRoot).ConfigureAwait(false);
            await _webDAV.EnsureFolderAsync($"{FilesRoot}/database").ConfigureAwait(false);
            await _webDAV.EnsureFolderAsync($"{FilesRoot}/media").ConfigureAwait(false);
            await _webDAV.EnsureFolderAsync(ManifestsRoot).ConfigureAwait(false);
        }

        private async Task<WebDavIncrementalBackupFile> UploadFileByHashAsync(string filePath, string kind)
        {
            await using var hashStream = File.OpenRead(filePath);
            string hash = await hashStream.CreateSHA256Async().ConfigureAwait(false);
            string extension = Path.GetExtension(filePath);
            string remotePath = $"{FilesRoot}/{kind}/{hash}{extension}";

            if (!await _webDAV.FileExistsAsync(remotePath).ConfigureAwait(false))
            {
                await using var uploadStream = File.OpenRead(filePath);
                await _webDAV.UploadAsync(remotePath, uploadStream).ConfigureAwait(false);
            }

            var fileInfo = new FileInfo(filePath);
            return new WebDavIncrementalBackupFile()
            {
                Kind = kind,
                Hash = hash,
                Name = Path.GetFileName(filePath),
                RemotePath = remotePath,
                Length = fileInfo.Length
            };
        }

        private static async Task<bool> FileHashEqualsAsync(string filePath, string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
            {
                return false;
            }

            await using var stream = File.OpenRead(filePath);
            string currentHash = await stream.CreateSHA256Async().ConfigureAwait(false);
            return string.Equals(currentHash, hash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
