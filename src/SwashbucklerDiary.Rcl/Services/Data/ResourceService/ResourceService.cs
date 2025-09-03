using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Services
{
    public class ResourceService : BaseDataService<ResourceModel>, IResourceService
    {
        protected readonly IResourceRepository _resourceRepository;

        protected readonly IMediaResourceManager _mediaResourceManager;

        protected readonly ISettingService _settingService;

        protected readonly IAppFileSystem _appFileSystem;

        public ResourceService(IResourceRepository resourceRepository,
            IMediaResourceManager mediaResourceManager,
            ISettingService settingService,
            IAppFileSystem appFileSystem)
        {
            base._iBaseRepository = resourceRepository;
            _resourceRepository = resourceRepository;
            _mediaResourceManager = mediaResourceManager;
            _settingService = settingService;
            _appFileSystem = appFileSystem;
        }

        public async Task DeleteUnusedResourcesAsync(Expression<Func<ResourceModel, bool>> expression)
        {
            await _resourceRepository.DeleteUnusedResourcesAsync(expression).ConfigureAwait(false);
        }

        public async Task DeleteAllUnusedResourcesWithFilesAsync()
        {
            await _resourceRepository.DeleteUnusedResourcesAsync(_ => true).ConfigureAwait(false);

            bool privacyMode = _settingService.GetTemp(it => it.PrivacyMode);
            var trulyUsedResourceUris = await _resourceRepository.QueryTrulyUsedResourcesAsync(privacyMode).ConfigureAwait(false);
            await DeleteResourceFiles(trulyUsedResourceUris).ConfigureAwait(false);
        }

        public Task<ResourceModel> FindIncludesAsync(string id)
        {
            return _resourceRepository.FindIncludesAsync(id);
        }

        public Task<List<Guid>> GetDiaryIdsAsync(string id)
        {
            return _resourceRepository.GetDiaryIdsAsync(id);
        }

        private async Task DeleteResourceFiles(List<string> resourceUris)
        {
            var filePaths = resourceUris
                .Select(it => _mediaResourceManager.RelativeUrlToFilePath(it))
                .Where(it => !string.IsNullOrEmpty(it))
                .ToList();
            foreach (var folderName in _mediaResourceManager.MediaResourceFolders.Values)
            {
                await _appFileSystem.ClearFolderAsync(Path.Combine(_appFileSystem.AppDataDirectory, folderName), filePaths).ConfigureAwait(false);
            }
        }
    }
}
